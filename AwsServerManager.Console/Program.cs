using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.EC2;
using Amazon.EC2.Model;
using AwsServerManager.Shared;

namespace AwsServerManager
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var argsDic = GetArgsDic(args);

				var itemsList = argsDic["--instances"];

				if (itemsList.StartsWith("\"") && itemsList.EndsWith("\""))
					itemsList = itemsList.Substring(1, itemsList.Length - 2);

				var items = itemsList.Split(',');

				var action = argsDic["--action"];

				using (var client = AwsUtil.CreateClient(AwsUtil.Region))
				{
					if (action == "start")
					{
						var request = new StartInstancesRequest();
						request.InstanceId.AddRange(items);
						client.StartInstances(request);
					}
					else if (action == "stop")
					{
						var request = new StopInstancesRequest();
						request.InstanceId.AddRange(items);
						client.StopInstances(request);
					}
				}
			}
			catch (ApplicationException exc)
			{
				Console.WriteLine(exc.Message);
			}
			catch (AmazonEC2Exception exc)
			{
				Console.WriteLine(exc.Message);
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}

		private static Dictionary<string, string> GetArgsDic(string[] args)
		{
			if (args.Length != 2)
				throw new ApplicationException(UsageText);

			var argsDic = new Dictionary<string, string>();
			foreach (var arg in args)
			{
				var parts = arg.Split('=');
				if (parts.Length != 2)
					throw new ApplicationException(UsageText);
				argsDic.Add(parts[0].ToLowerInvariant(), parts[1]);
			}
			return argsDic;
		}

		private const string UsageText = "Usage:\r\naws-dev-server-manager-cli --action=stop --instances=\"id1,id2\"";
	}
}
