using NICE.Feeds.Models.Indev.Detail;

namespace Comments.Common
{
	public static class Routes
	{
		public static class External
		{
			public const string HomePage = "/";
			public const string InconsultationListPage = "/guidance/inconsultation";

			#region ConsultationUrls - this code has been copied from guidance-web

			public static string ConsultationUrl(ConsultationDetail consultationDetail)
			{
				if (!string.IsNullOrEmpty(consultationDetail.OrigProjectReference))
					return LocalRouteForUpdateConsultations(consultationDetail);

				var isProposed = false; //TODO: figure out whether this consultation is on a proposed product.

				return LocalRouteForConsultation(consultationDetail.Reference, consultationDetail.ResourceTitleId,
					isProposed);
			}

			public static string LocalRouteForUpdateConsultations(ConsultationDetail consultationProduct)
			{
				return string.Format("/guidance/{0}/update/{1}/consultation/{2}",
					consultationProduct.OrigProjectReference,
					consultationProduct.Reference, consultationProduct.ResourceTitleId).ToLower();
			}

			private static string LocalRouteForConsultation(string reference, string resourceTitleId, bool isProposed)
			{
				if (IsGID(reference))
				{
					string statusSlug = isProposed
						? "proposed"
						: "indevelopment";
					return string.Format("/guidance/{0}/{1}/consultation/{2}",
						statusSlug,
						reference.ToLowerInvariant(),
						resourceTitleId);
				}

				return string.Format("/guidance/{0}/consultation/{1}", reference.ToLowerInvariant(), resourceTitleId);
			}

			private static bool IsGID(string id)
			{
				return id.ToLowerInvariant().StartsWith("gid-");
			}

			#endregion ConsultationUrls

		}

		/// <summary>
		/// These routes are for the front-end only. the front-end has a base element set with an href of "/consultations/", so these routes are relative to that.
		/// </summary>
		public static class Internal
		{
			public const string DownloadPageRoute = "/";
		}
	}
}
