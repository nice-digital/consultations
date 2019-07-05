using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public partial class KeyPhrase
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int KeyPhraseId { get; set; }

		public string Text { get; set; }

		public ICollection<CommentKeyPhrase> CommentKeyPhrase { get; set; }
		public ICollection<AnswerKeyPhrase> AnswerKeyPhrase { get; set; }
	}
}
