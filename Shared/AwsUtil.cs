using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace AwsServerManager.Shared
{
	public static class AwsUtil
	{
		public static AmazonEC2Client CreateClient(RegionEndpoint region)
		{
			var key = ConfigurationManager.AppSettings["AmazonKey"];
			var secretkey = ConfigurationManager.AppSettings["AmazonSecretKey"];
			if (!string.IsNullOrEmpty(key))
				return new AmazonEC2Client(key, secretkey, region);
			else
				return new AmazonEC2Client();
		}

		public static RegionEndpoint Region
		{
			get
			{
				var name = ConfigurationManager.AppSettings["AmazonRegion"];
				if (string.IsNullOrEmpty(name))
					return RegionEndpoint.USEast1;

				return RegionEndpoint.GetBySystemName(name);
			}
		}
	}
}
