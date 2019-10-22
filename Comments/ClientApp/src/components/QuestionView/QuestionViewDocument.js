import pdfMake from 'pdfmake/build/pdfmake';
import vfsFonts from 'pdfmake/build/vfs_fonts';

var moment = require('moment');
moment().format();

const _format = (questions) => {
	return questions.map(item => {
		return ([
            {text: item.questionText},
            '\n',
		]);
	});
}

export const CreateQuestionPdf = (questionsForPDF, titleForPDF, endDate) => {
	const {vfs} = vfsFonts.pdfMake;
    pdfMake.vfs = vfs;
    
    const questions = questionsForPDF;
    const formattedQuestions = _format(questions);
    const consultationEndDate = moment(endDate).format("D MMMM YYYY");

    const documentDefinition = {
            info: {
                title: 'Questions for "' + titleForPDF +'"',
            },
            pageSize: 'A4',
            pageOrientation: 'portrait',
            pageMargins: 60,
            content: [
                {
                    image: 'data:image/png;base64,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',
                    width: 200
                },
                '\n',
                '\n',
                {text: titleForPDF, style: 'header'},
                '\n',
                {text: 'This consultation includes the following questions:'},
                '\n',
                {
                    ul: formattedQuestions
                },
                '\n',
                {text: 'Please note: Responses to this consultation need to be submitted online. This consultation closes on ' + consultationEndDate + "."},
            ],
        	styles: {
                header: {
                    bold: true,
                    fontSize: 16
                }
            },
            defaultStyle: {
                fontSize: 12
            }
    };

	pdfMake.createPdf(documentDefinition).open();
}