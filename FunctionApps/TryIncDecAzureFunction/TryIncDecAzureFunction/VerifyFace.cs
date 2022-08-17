using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web;


using System.Net;
using System.Text;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;

namespace TryIncDecAzureFunction
{
	public static class VerifyFace
	{
		private static readonly HttpClient client = new HttpClient();
		/****************/
		private static string[] faceDescriptions;
		private static IList<DetectedFace> faceList;
		private static string subscriptionKey = "221d3c4482144c4ba83a9812a7113fb8";
		private static string faceEndpoint = "https://tryfaceapiinstance.cognitiveservices.azure.com/";
		private static readonly IFaceClient faceClient = new FaceClient(
			new ApiKeyServiceClientCredentials(subscriptionKey),
			new System.Net.Http.DelegatingHandler[] { });

		private static string KNOWN_KEY = "brurya";
		
		static VerifyFace()
        {
			faceDescriptions = new string[0];
			faceList = new List<DetectedFace>();
			faceClient.Endpoint = faceEndpoint;
		}
		
		/****************/

		public static Stream getImageFromRequest(HttpRequest req)
        {
			string imgBase64Enc = req.Query["imgBase64Enc"];

			Stream imgStream;

			if (string.IsNullOrEmpty(imgBase64Enc))
			{
				var form = req.Form;
				var files = form.Files;

				if (files.Count > 0)
				{
					var img = files[0];
					imgStream = img.OpenReadStream();
				}
				else
				{
					imgStream = null;
				}
			}
			else
			{
				imgStream = Base64EncodingToStream(imgBase64Enc);
			}

			return imgStream;
		}

		public static async Task<string> personIdToJson(string personGroupId, Guid guid)
        {
			Person person;
			if (guid == Guid.Empty)
			{
				person = new Person(guid, name: "nobody", userData: @"{""firstName"" : ""nobody"", ""lastName"" : ""nobody""}");
			}
			else
			{
				person = await faceClient.PersonGroupPerson.GetAsync(personGroupId, guid);
			}
			string jsonString = JsonConvert.SerializeObject(person);
			jsonString = jsonString.Replace("\"{", "{");
			jsonString = jsonString.Replace("\\\"","\"");
			jsonString = jsonString.Replace("}\"","}");
			
			return jsonString;
		}

		[FunctionName("GetPersonDetails")]
		public static async Task<string> GetPersonDetailsHTTP(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			/*
			 *	input - 
			 *			query: 
			 *				@personGroupId
			 *				@personId
			 * 
			 *	output -
			 *			finding the details of @personId in @personGroupId
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");
			string personGroupId = req.Query["personGroupId"];
			string personId = req.Query["personId"];


			return await personIdToJson(personGroupId, new Guid(personId));
		}

		[FunctionName("TryToMatch")]
		public static async Task<Guid> TryToMatchHTTP(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			/*
			 *	input - 
			 *			query: @personGroupId [@imgBase64Enc]
			 *			body(files): @image
			 * 
			 *	output -
			 *			finding the best match in @personGroupId for @image (if @imgBase64Enc is not provided)
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");
			string personGroupId = req.Query["personGroupId"];

			Stream imgStream = getImageFromRequest(req);
			imgStream.Position = 0;
			if (imgStream == null)
            {
				return Guid.Empty;
            }
			return await TryToMatch(personGroupId, imgStream);
		}

		[FunctionName("TryToMatchDetails")]
		public static async Task<string> TryToMatchDetailsHTTP(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			/*
			 *	input - 
			 *			query: @personGroupId [@imgBase64Enc]
			 *			body(files): @image
			 * 
			 *	output -
			 *			finding the details of the best match in @personGroupId for @image (if @imgBase64Enc is not provided)
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");
			string personGroupId = req.Query["personGroupId"];

			Stream imgStream = getImageFromRequest(req);
			imgStream.Position = 0;
			if (imgStream == null)
			{
				return string.Empty;
			}
			Guid guid = await TryToMatch(personGroupId, imgStream);
			var jsonString = await personIdToJson(personGroupId, guid);

			return jsonString;
		}


		[FunctionName("CreatePerson")]
		public static async Task<Person> CreatePersonHTTP(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			/*
			 *	input - 
			 *			query/body:	
			 *				@username
			 *				@personGroupId
			 *				@jsonData - data about the person packed in a string representing a json
			 *				
			 *	output -
			 *			@person - the person that was created, of type persons
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");

			string name = req.Query["username"];
			string personGroupId = req.Query["personGroupId"];
			string jsonData = req.Query["jsonData"];
			string auth = req.Query["auth"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);
			
			name = name ?? data?.name;
			personGroupId = personGroupId ?? data?.personGroupId;
			jsonData = jsonData ?? data?.jsonData;
			auth = auth ?? data?.auth;

			Person newPerson = await CreatePerson(personGroupId, name, jsonData, auth);
			return newPerson;
		}
		[FunctionName("CreateAdmin")]
		public static async Task CreateAdminHTTP(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			/*
			 *	input - 
			 *			query/body:	
			 *				@username
			 *				@personGroupId
			 *				@jsonData - data about the person packed in a string representing a json
			 *				
			 *	output -
			 *			@person - the person that was created, of type persons
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");

			string name = req.Query["username"];
			string auth = req.Query["auth"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);

			name = name ?? data?.name;
			auth = auth ?? data?.auth;

			await CreateAdmin(name, auth);
		}

		[FunctionName("AddFaceToPerson")]
		public static async Task AddFaceToPersonHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{
			/*
			 *	input - 
			 *			query:
			 *				@guid - guid of specific person
			 *				@personGroupId
			 *				[@imgBase64Enc]
			 *			body[files]:
			 *				@image
			 *	
			 *	adds @image to the person represented by @guid in @personGroupId (if @imgBase64Enc is not given)			
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");

			string guid = req.Query["guid"];
			string personGroupId = req.Query["personGroupId"];

			Stream imgStream = getImageFromRequest(req);

			if (imgStream == null)
			{
				return;
			}

			await AddFaceToPersonStream(personGroupId, new Guid(guid), imgStream);
			await faceClient.PersonGroup.TrainAsync(personGroupId);
		}

		[FunctionName("ListAllPersons")]
		public static async Task<string> ListAllPersonsHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{
			/*
			 *	input - 
			 *			query:
			 *				@key - to make sure that not anyone can get this list
			 *				@personGroupId
			 *	
			 *	json of all Person objects in @personGroupId (if @imgBase64Enc is not given)			
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");

			string key = req.Query["key"];
			string personGroupId = req.Query["personGroupId"];
			if (key != KNOWN_KEY)
            {
				//problem
				return null;
            }
			IList<Person> persons = await GetPersonsInPersonGroup(personGroupId);
			//convert the list to json
			var jsonList = JsonConvert.SerializeObject(persons);

			return jsonList;

		}
		[FunctionName("ListAllAdmins")]
		public static async Task<string> ListAllAdminsHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{
			/*
			 *	input - 
			 *			query:
			 *				@key - to make sure that not anyone can get this list
			 *				@personGroupId
			 *	
			 *	json of all Person objects in @personGroupId (if @imgBase64Enc is not given)			
			 */
			log.LogInformation("C# HTTP trigger function processed a request.");

			string key = req.Query["key"];
			if (key != KNOWN_KEY)
			{
				//problem
				return null;
			}
			var adminsJson = await SqlCommunication.SqlSelectAdmins();
			
			return adminsJson;

		}


		[FunctionName("ListAllPersonGroups")]
		public static async Task<string> ListAllPersonGroupsHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{
			
			log.LogInformation("C# HTTP trigger function processed a request.");

			string key = req.Query["key"];
			if (key != KNOWN_KEY)
			{
				//problem
				return null;
			}
			IList<PersonGroup> personGroups = await GetPersonGroupsAsync();
			//convert the list to json
			var jsonList = JsonConvert.SerializeObject(personGroups);

			return jsonList;

		}

		[FunctionName("CreatePersonGroup")]
		public static async Task CreatePersonGroupHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{

			log.LogInformation("C# HTTP trigger function processed a request.");

			string personGroupId = req.Query["personGroupId"];
			string personGroupName = req.Query["personGroupName"];

			await CreatePersonGroup(personGroupId, personGroupName);
			

		}

		[FunctionName("DeletePersonGroup")]
		public static async Task DeletePersonGroupHTTP(
	[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
	ILogger log)
		{

			log.LogInformation("C# HTTP trigger function processed a request.");

			string personGroupId = req.Query["personGroupId"];

			await DeletePersonGroup(personGroupId);


		}
		/**********************************/

		private static async Task<string[]> DetectedFacesToStrings(string filePath)
		{
			faceList = await UploadAndDetectFaces(filePath);

			faceDescriptions = new String[faceList.Count];

			for (int i = 0; i < faceList.Count; ++i)
			{
				DetectedFace face = faceList[i];

				// Store the face description.
				faceDescriptions[i] = FaceDescription(face);
			}

			return faceDescriptions;
		}

		private static Stream getStreamFromUri(string uri)
		{
			Uri u = new Uri(uri);
			if (u.IsFile)
			{
				return File.OpenRead(uri);
			}
			WebClient webClient = new WebClient();
			byte[] data = webClient.DownloadData(uri);
			return new MemoryStream(data);
		}

		private static async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath)
		{
			// The list of Face attributes to return.
			IList<FaceAttributeType> faceAttributes =
				new FaceAttributeType[]
				{
			FaceAttributeType.Gender,
			FaceAttributeType.Emotion,
			FaceAttributeType.Smile,
			FaceAttributeType.Glasses,
			FaceAttributeType.Age,
			FaceAttributeType.Hair
				};

			// Call the Face API.
			try
			{
				using (Stream imageFileStream = getStreamFromUri(imageFilePath))
				{
					IList<DetectedFace> faceList = await faceClient.Face.DetectWithStreamAsync(imageFileStream, true, false, faceAttributes);
					return faceList;
				}
			}
			catch (APIErrorException f)
			{
				Console.WriteLine(f.Message);
				return new List<DetectedFace>();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message, "Error occurred");
				return new List<DetectedFace>();

			}
		}

		private static async Task<IList<DetectedFace>> UploadAndDetectFacesStream(Stream image)
		{
			// The list of Face attributes to return.
			IList<FaceAttributeType> faceAttributes =
				new FaceAttributeType[]
				{
			FaceAttributeType.Gender,
			FaceAttributeType.Emotion,
			FaceAttributeType.Smile,
			FaceAttributeType.Glasses,
			FaceAttributeType.Age,
			FaceAttributeType.Hair
				};

			// Call the Face API.
			try
			{
				image.Position = 0;
				using (Stream imageFileStream = image)
				{

					IList<DetectedFace> faceList = await faceClient.Face.DetectWithStreamAsync(imageFileStream, true, false, faceAttributes);
					return faceList;
				}
			}
			catch (APIErrorException f)
			{
				Console.WriteLine(f.Message);
				return new List<DetectedFace>();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message, "Error occurred");
				return new List<DetectedFace>();

			}
		}

		// Creates a string out of the attributes describing the face.
		private static string FaceDescription(DetectedFace face)
		{
			StringBuilder sb = new StringBuilder();

			// Add the gender, age, and smile.
			sb.Append($"Gender: {face.FaceAttributes.Gender}\n");
			sb.Append($"Age: {face.FaceAttributes.Age}\n");
			sb.Append(String.Format("Smile {0:F1}% ", face.FaceAttributes.Smile * 100));

			// Add glasses.
			sb.Append($"\n{face.FaceAttributes.Glasses}\n");

			// Add the emotions. Display all emotions over 10%.
			sb.Append("Emotion: ");
			Emotion emotionScore = face.FaceAttributes.Emotion;
			if (emotionScore.Anger >= 0.1f) sb.Append(
				String.Format("anger {0:F1}%, ", emotionScore.Anger * 100));
			if (emotionScore.Contempt >= 0.1f) sb.Append(
				String.Format("contempt {0:F1}%, ", emotionScore.Contempt * 100));
			if (emotionScore.Disgust >= 0.1f) sb.Append(
				String.Format("disgust {0:F1}%, ", emotionScore.Disgust * 100));
			if (emotionScore.Fear >= 0.1f) sb.Append(
				String.Format("fear {0:F1}%, ", emotionScore.Fear * 100));
			if (emotionScore.Happiness >= 0.1f) sb.Append(
				String.Format("happiness {0:F1}%, ", emotionScore.Happiness * 100));
			if (emotionScore.Neutral >= 0.1f) sb.Append(
				String.Format("neutral {0:F1}%, ", emotionScore.Neutral * 100));
			if (emotionScore.Sadness >= 0.1f) sb.Append(
				String.Format("sadness {0:F1}%, ", emotionScore.Sadness * 100));
			if (emotionScore.Surprise >= 0.1f) sb.Append(
				String.Format("surprise {0:F1}% ", emotionScore.Surprise * 100));
			return sb.ToString();
		}

		private static async Task<string> UriToFaceDescriptionString(string imagePath)
		{

			string[] faces = await DetectedFacesToStrings(imagePath);

			string res = "";
			foreach (var face in faces)
			{
				res += face.ToString();
				res += "\n\n";
			}

			return res;

		}

		private static async Task CreatePersonGroup(string personGroupId, string personGroupName)
		{
			await faceClient.PersonGroup.CreateAsync(personGroupId, personGroupName);
			SqlCommunication.CreatePersonGroupTables(personGroupId);
		}

		private static async Task DeletePersonGroup(string personGroupId)
        {
			await faceClient.PersonGroup.DeleteAsync(personGroupId);
			SqlCommunication.DeletePersonGroupTables(personGroupId);
        }
		private static async Task<Person> CreatePerson(string personGroupId, string userName, string jsonData, string authString)
		{
			var person = await faceClient.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: userName, userData: jsonData);
			string tablename = SqlCommunication.GetTableName(SqlCommunication.AUTH_TABLE_NAME, personGroupId);
			Auth auth = new Auth();
			auth.userid = person.PersonId;
			auth.username = person.Name;
			auth.pwauth = authString;
			SqlCommunication.InsertIntoQuery(tablename,auth);

			return person;
		}
		private static async Task CreateAdmin(string userName, string authString)
		{
			string tablename = SqlCommunication.GetTableName(SqlCommunication.AUTH_TABLE_NAME, "admin");
			Auth auth = new Auth();
			auth.username = userName;
			auth.pwauth = authString;
			SqlCommunication.InsertIntoQuery(tablename, auth);
		}


		private static async Task AddFaceToPerson(string personGroupId, Guid personId, string uri)
		{
			using (Stream stream = getStreamFromUri(uri))
			{
				await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, personId, stream);
			}
		}
		private static async Task AddFaceToPerson(string personGroupId, Person person, string uri)
		{
			Guid personId = person.PersonId;
			using (Stream stream = getStreamFromUri(uri))
			{
				await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, personId, stream);
			}
		}

		private static async Task AddFaceToPersonStream(string personGroupId, Guid personId, Stream image)
		{
			using (Stream stream = image)
			{
				await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, personId, stream);
			}
		}
		private static async Task AddFaceToPersonStream(string personGroupId, Person person, Stream image)
		{
			Guid personId = person.PersonId;
			using (Stream stream = image)
			{
				await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, personId, stream);
			}
		}

		private static DetectedFace GetMostDominantFace(IList<DetectedFace> faces)
		{
			int max_area = 0;
			DetectedFace? max_face = null;

			if (faces.Count == 0) return null;
			for (int i = 0; i < faces.Count; i++)
			{
				DetectedFace cur_face = faces[i];
				int cur_area = cur_face.FaceRectangle.Height * cur_face.FaceRectangle.Width;
				if (cur_area > max_area)
				{
					max_face = cur_face;
					max_area = cur_area;
				}
			}

			return max_face ?? new DetectedFace();
		}

		private static async Task AddPerson(string personGroupId, string userName, string jsonData,string auth, int imgCount)
		{

			Person person = await CreatePerson(personGroupId, userName, jsonData,auth);
			string base_path = @"C:\Users\anukh\Desktop\faces\";

			string uri;
			for (int i = 0; i < imgCount; i++)
			{
				uri = $"{base_path}{userName}\\{i}.jpeg";

				await AddFaceToPerson(personGroupId, person, uri);
			}
		}

		private static async Task<IList<Person>> GetPersonsInPersonGroup(string personGroupId)
		{
			return await faceClient.PersonGroupPerson.ListAsync(personGroupId);
		}

		private static async Task<IList<Guid>> GetPersonGuidsInPersonGroup(string personGroupId)
		{
			IList<Person> persons = await faceClient.PersonGroupPerson.ListAsync(personGroupId);

			IList<Guid> personGuids = new List<Guid>();

			foreach (Person person in persons)
			{
				foreach (Guid face in person.PersistedFaceIds)
				{
					personGuids.Add(face);
				}
			}

			return personGuids;
		}
		private static string StringifyPerson(Person person)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Name: ");
			sb.AppendLine(person.Name);

			sb.Append("Guid: ");
			sb.AppendLine(person.PersonId.ToString());

			sb.Append("Num of Photos: ");
			sb.AppendLine(person.PersistedFaceIds.Count.ToString());

			return sb.ToString();
		}

		private static async Task PrintPersonGroup(string personGroupId)
		{
			IList<Person> l = await GetPersonsInPersonGroup(personGroupId);
			foreach (Person person in l)
			{
				Console.WriteLine(StringifyPerson(person));
				Console.WriteLine();
			}
		}
		private static async Task<IList<IdentifyCandidate>> IdentifyImage(string personGroupId, string img)
		{
			IList<DetectedFace> detectedFaces = await UploadAndDetectFaces(img);
			IList<IdentifyResult> results;
			IList<Guid> faceIds = new List<Guid>();
			DetectedFace face = GetMostDominantFace(detectedFaces);
			faceIds.Add(face.FaceId.Value);
			if (detectedFaces.Count > 0)
			{
				results = await faceClient.Face.IdentifyAsync(faceIds, personGroupId);
				// Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
				//similarResults = await faceClient.Face.FindSimilarAsync(detectedFaces[0]?.FaceId, null, null, targetFaceIds);
			}
			else
			{
				results = new List<IdentifyResult>();
			}


			IdentifyResult? result = results.Count > 0 ? results[0] : null;
			return result?.Candidates ?? new List<IdentifyCandidate>();
		}

		private static async Task<IList<IdentifyCandidate>> IdentifyImageStream(string personGroupId, Stream img)
		{
			IList<DetectedFace> detectedFaces = await UploadAndDetectFacesStream(img);
			IList<IdentifyResult> results;
			IList<Guid> faceIds = new List<Guid>();
			DetectedFace face = GetMostDominantFace(detectedFaces);
			if (face==null) return null;
			faceIds.Add(face.FaceId.Value);
			if (detectedFaces.Count > 0)
			{
				results = await faceClient.Face.IdentifyAsync(faceIds, personGroupId);
				// Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
				//similarResults = await faceClient.Face.FindSimilarAsync(detectedFaces[0]?.FaceId, null, null, targetFaceIds);
			}
			else
			{
				results = new List<IdentifyResult>();
			}


			IdentifyResult result = results.Count > 0 ? results[0] : null;
			return result?.Candidates ?? new List<IdentifyCandidate>();
		}

		private static async Task<string> StrigifyIndetifyCandidate(string personGroupId, IdentifyCandidate candidate)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Guid: ");
			sb.AppendLine(candidate.PersonId.ToString());
			string name = (await faceClient.PersonGroupPerson.GetAsync(personGroupId, candidate.PersonId)).Name;
			sb.Append("Name: ");
			sb.AppendLine(name);

			sb.Append("Confidence: ");
			sb.AppendLine(candidate.Confidence.ToString());

			return sb.ToString();
		}


		private static Stream Base64EncodingToStream(string base64encode)
		{
			byte[] bytes = Convert.FromBase64String(base64encode);
			return new MemoryStream(bytes);
		}

		private static async Task<IdentifyCandidate> GetBestCandidate(string personGroupId, IList<IdentifyCandidate> candidates)
        {
			double threshold = .5;

			IdentifyCandidate bestCandidate = candidates.Count > 0 ? candidates[0] : null;
			foreach (var candidate in candidates)
            {
				if (candidate.Confidence > bestCandidate.Confidence)
                {
					bestCandidate = candidate;
                }
            }
			if (bestCandidate != null && bestCandidate.Confidence < threshold) { bestCandidate = null; };
			
			return bestCandidate;
        }

		private static async Task<Guid> TryToMatch(string personGroupId, Stream img)
        {
			IList<IdentifyCandidate> candidates = await IdentifyImageStream(personGroupId, img);
			if (candidates == null) return Guid.Empty;
			IdentifyCandidate res = await GetBestCandidate(personGroupId, candidates);
			if (res == null)
            {
				return Guid.Empty;
            }
			return res.PersonId;
        }


		private static async Task<IList<PersonGroup>> GetPersonGroupsAsync()
        {
			IList<PersonGroup> personGroups = new List<PersonGroup>();
			return await faceClient.PersonGroup.ListAsync();
        }


	}
}
