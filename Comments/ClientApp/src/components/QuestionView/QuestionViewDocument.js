import pdfMake from "pdfmake/build/pdfmake";
import niceLogoBase64 from './nice-logo.png';
var moment = require('moment');

export const createQuestionPdf = (questionsForPDF, titleForPDF, endDate) => {
	// webpack seems to have an issue with importing the vfs (virtual file system) fonts file from the package
	// so had to import the defaults font via cdn
	// https://pdfmake.github.io/docs/fonts/custom-fonts-client-side/
	pdfMake.fonts = {
		Roboto: {
			normal: 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-Regular.ttf',
			bold: 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-Medium.ttf',
			italics: 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-Italic.ttf',
			bolditalics: 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-MediumItalic.ttf'
		}
	};

	const documentDefinition = createDocumentDefinition(questionsForPDF, titleForPDF, endDate);
	pdfMake.createPdf(documentDefinition).open();
};

export const createDocumentDefinition = (questionsForPDF, titleForPDF, endDate) => {

    const formattedQuestions = questionsForPDF.map(item => [
            {text: item.questionText},
            '\n',
		]
    );

    const consultationEndDate = moment(endDate).format("D MMMM YYYY");

    return {
            info: {
                title: `Questions for "${titleForPDF}"`,
            },
            pageSize: 'A4',
            pageOrientation: 'portrait',
            pageMargins: 60,
            content: [
                {
                    image: niceLogoBase64,
                    width: 250
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
};
