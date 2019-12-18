namespace ImageDescriber
{
	#region

	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
	using Microsoft.Extensions.Options;

	#endregion

	public class ImageAnalyzer
	{
		private static readonly List<VisualFeatureTypes> Features =
			new List<VisualFeatureTypes>
			{
				VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
				VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
				VisualFeatureTypes.Tags
			};

		private readonly string _computerVisionEndpoint;
		private readonly string _computerVisionKey;

		public ImageAnalyzer(IOptions<Credentials> options)
		{
			_computerVisionKey = options.Value.ComputerVisionKey;
			_computerVisionEndpoint = options.Value.ComputerVisionEndpoint;
		}

		public static ComputerVisionClient Authenticate(string endpoint, string key)
		{
			var client =
				new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
					{Endpoint = endpoint};
			return client;
		}

		public async Task<ImageAnalysis> AnalyzeImageAsync(Stream image)
		{
			var client = Authenticate(_computerVisionEndpoint, _computerVisionKey);

			var analysis = await client.AnalyzeImageInStreamAsync(image, Features);
			return analysis;
		}

		public async Task<ImageAnalysis> AnalyzeUrl(string url)
		{
			var client = Authenticate(_computerVisionEndpoint, _computerVisionKey);
			var result = await client.AnalyzeImageWithHttpMessagesAsync(url, Features);
			return result.Body;
		}
	}
}