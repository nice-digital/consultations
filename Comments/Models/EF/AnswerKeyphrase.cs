using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models.EF
{
	public class AnswerKeyPhrase
	{
		public int AnswerKeyPhraseId { get; set; }
		public int KeyPhraseId { get; set; }
		public float Score { get; set; }
	}
}
