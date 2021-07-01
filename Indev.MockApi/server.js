require('dotenv').config();
const jsonServer = require('json-server');
const http = require('http');

const http_port = process.env.HTTP_PORT || 3001;

const rewriter = jsonServer.rewriter({
	'/consultation-comments-list': '/consultation-comments-list' ,
	'/consultation-comments/:consultationId' : '/comments?ConsultationId=:consultationId',
	'/consultation-comments/:consultationId/document/:documentId/chapter-slug/:slug' : '/chapters?ConsultationId=:consultationId&ConsultationDocumentId=:documentId&Slug=:slug',
});

const server = jsonServer.create();
const middleware = jsonServer.defaults();
const router = jsonServer.router('db.json');

server.use(rewriter);
server.use(middleware);
server.use(router);

http.createServer(server).listen(http_port, () => {
  console.log(`Indev Mock API is running on http://localhost:${http_port}`);
});
