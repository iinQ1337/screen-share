const express = require('express');
const http = require('http');
const WebSocket = require('ws');
const app = express();

const server = http.createServer(app);

const wss = new WebSocket.Server({ server });

app.use(express.static('public'));

wss.on('connection', (ws) => {
    console.log('WebSocket connected');

    ws.on('message', (message) => {
        const binaryData = Buffer.from(message, 'base64');
        
        ws.send(binaryData);
    });
});

server.listen(3000, () => {
    console.log(`HTTP and WebSocket servers are running on port ${server.address().port}`);
});
