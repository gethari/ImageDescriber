// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

namespace ImageDescriber.Bots
{
	#region

	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
	using Microsoft.Bot.Builder;
	using Microsoft.Bot.Schema;

	#endregion

	public class EchoBot : ActivityHandler
	{
		private readonly HttpClient _httpClient;
		private readonly ImageAnalyzer _imageAnalyzer;

		public EchoBot(ImageAnalyzer imageAnalyzer)
		{
			_imageAnalyzer = imageAnalyzer;
			_httpClient = new HttpClient();
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
			CancellationToken cancellationToken)
		{
			var result = await DoSomething(turnContext);
			await turnContext.SendActivityAsync(result, cancellationToken: cancellationToken);
		}

		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
			ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			var welcomeText =
				"Hello and welcome to ImageDescriber Bot. Enter a image url or upload a image to begin analysis :";
			foreach (var member in membersAdded)
				if (member.Id != turnContext.Activity.Recipient.Id)
					await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText), cancellationToken);
		}

		private async Task<string> DoSomething(ITurnContext<IMessageActivity> turnContext)
		{
			var result = new ImageAnalysis();
			if (turnContext.Activity.Attachments?.Count > 0)
			{
				var attachment = turnContext.Activity.Attachments[0];
				if (attachment.ContentType == "image/jpeg" || attachment.ContentType == "image/png")
				{
					var image = await _httpClient.GetStreamAsync(attachment.ContentUrl);
					if (image != null) result = await _imageAnalyzer.AnalyzeImageAsync(image);
				}
			}
			else
			{
				result = await _imageAnalyzer.AnalyzeUrl(turnContext.Activity.Text);
			}

			var stringResponse =
				$"I think the Image you uploaded is a {result.Tags[0].Name.ToUpperInvariant()} and it is {result.Description.Captions[0].Text.ToUpperInvariant()} ";
			return stringResponse;
		}
	}
}