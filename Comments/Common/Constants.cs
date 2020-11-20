namespace Comments.Common
{
    public static class Constants
    {
        public const string ConsultationsBasePath = "/consultations";
	    public const string ConsultationsReplaceableRelativeUrl = ConsultationsBasePath + "/{0}/{1}/{2}";
	    public const string ConsultationsPreviewReplaceableRelativeUrl = ConsultationsBasePath + "/preview/{0}/consultation/{1}/document/{2}/chapter/{3}";
		public const string ErrorPath = "/error";
	    public const string StatusAPIKeyName = "ECStatusApiKey";
		
	    /// <summary>
		/// This document number is here to pass to indev to call the "Draft preview detail" feed. The preview feed cannot be called without a document id, and if you do pass one
		/// all it sets is the SelectedDocumentId and the DocumentId properties on the returned json.
		/// </summary>
		public const int DummyDocumentNumberForPreviewProject = 0;

	    public static class AppSettings
	    {
		    public const string Keyword = "Keyword";
	    }

	    public static class Export
	    {
		    public const string SheetName = "Comments";
		    public const string ExpressionOfInterestColumnDescription = "Organisation interested in formal support";
		    public const string Yes = "Yes";
		    public const string No = "No";
		}

	    public static class Auth
	    {
		    public const string ControllerName = "Account";
		    public const string LoginAction = "Login";
		    public const string LogoutAction = "Logout";
	    }

	    public static class CollationCode
	    {
			/// <summary>
			/// we show to the user chunked with space
			/// </summary>
			public const string RegExChunkedWithSpaces = @"\d{4}\s\d{4}\s\d{4}";

			/// <summary>
			/// after they enter the code, we remove spaces.
			/// </summary>
			public const string RegExSpacesRemoved = @"\d{12}";
	    }
	}
}
