using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public partial class CommentKeyPhrase
	{
		public int CommentKeyPhraseId { get; set; }
		public int CommentId { get; set; }
		public int KeyPhraseId { get; set; }
		public float Score { get; set; }
	}
}
