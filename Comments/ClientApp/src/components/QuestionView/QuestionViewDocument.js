import pdfMake from 'pdfmake/build/pdfmake';
import vfsFonts from 'pdfmake/build/vfs_fonts';
import niceLogoBase64 from './nice-logo.png';

var moment = require('moment');


export const createQuestionPdf = (questionsForPDF, titleForPDF, endDate) => {
	const {vfs} = vfsFonts.pdfMake;
    pdfMake.vfs = vfs;
    const documentDefinition = createDocumentDefinition(questionsForPDF, titleForPDF, endDate);
    pdfMake.createPdf(documentDefinition).open();

}

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
}