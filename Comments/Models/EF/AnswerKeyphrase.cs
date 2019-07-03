using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
	public partial class AnswerKeyPhrase
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int AnswerKeyPhraseId { get; set; }

		public int AnswerId { get; set; }
		public int KeyPhraseId { get; set; }
		public float Score { get; set; }

		public Answer Answer { get; set; }
		public KeyPhrase KeyPhrase { get; set; }
	}
}
