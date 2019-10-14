import pdfMake from 'pdfmake/build/pdfmake';
import vfsFonts from 'pdfmake/build/vfs_fonts';



const _format = (questions) => {
	return questions.map(item => {
		return ([
			{text: item.questionText},
		]);
	});
}

export default (questionsForPDF) => {
	const {vfs} = vfsFonts.pdfMake;
    pdfMake.vfs = vfs;
    
    const questions = questionsForPDF;
    const formattedQuestions = _format(questions);

    const documentDefinition = {
            pageSize: 'A4',
            pageOrientation: 'portrait',
            pageMargins: 60,
            content: [
                {text: 'NICE', style: 'header'},
                {text: 'TITLE of consultation', style: 'header'},
                '\n',
                {text: 'The consultation includes the following questions:'},
                '\n',
                {
                    ul: formattedQuestions
                }
            ],
        	styles: {
                header: {
                    bold: true,
                    fontSize: 15
                }
            },
            defaultStyle: {
                fontSize: 12
            }
    };

	pdfMake.createPdf(documentDefinition).open();
}