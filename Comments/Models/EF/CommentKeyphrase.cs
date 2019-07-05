using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public partial class CommentKeyPhrase
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CommentKeyPhraseId { get; set; }

		public int CommentId { get; set; }
		public int KeyPhraseId { get; set; }
		public float Score { get; set; }

		public Comment Comment { get; set; }
		public KeyPhrase KeyPhrase { get; set; }
	}
}
