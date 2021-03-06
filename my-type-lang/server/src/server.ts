import { TypeParser } from './parser';
/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
'use strict';

import {
	IPCMessageReader, IPCMessageWriter, createConnection, IConnection, TextDocuments, TextDocument,
	InitializeResult, TextDocumentPositionParams, CompletionItem,

} from 'vscode-languageserver';
import { setupEnv } from './primitivesProvider';

import Uri from 'vscode-uri'

import * as path from 'path';
import * as fs from 'fs'

const typeEngine = setupEnv()
const easyTypesParser = new TypeParser()
// Create a connection for the server. The connection uses Node's IPC as a transport
let connection: IConnection = createConnection(new IPCMessageReader(process), new IPCMessageWriter(process));

// Create a simple text document manager. The text document manager
// supports full document sync only
let documents: TextDocuments = new TextDocuments();
// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// After the server has started the client sends an initialize request. The server receives
// in the passed params the rootPath of the workspace plus the client capabilities. 

connection.onInitialize((_): InitializeResult => {


	return {
		capabilities: {
			// Tell the client that the server works in FULL text document sync mode
			textDocumentSync: documents.syncKind,
			// Tell the client that the server support code complete
			completionProvider: {
				resolveProvider: true
			}
		}
	}
});

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent((change) => {
	let fspath = Uri.parse(change.document.uri).fsPath;
	const ext = path.extname(fspath)
	let lines: string[]
	lines = change.document.getText().split(/\r?\n/g)
	switch (ext) {
		case '.ty':
			easyTypesParser.parse(fspath, lines)
			break

	}


	// validateTextDocument(change.document);
});

// The settings interface describe the server relevant settings part
// interface Settings {
// 	lspSample: ExampleSettings;
// }

// These are the example settings we defined in the client's package.json
// // file
// interface ExampleSettings {
// 	maxNumberOfProblems: number;
// }


// The settings have changed. Is send on server activation
// as well.
connection.onDidChangeConfiguration((_) => {
	// let settings = <Settings>change.settings;
	// maxNumberOfProblems = settings.lspSample.maxNumberOfProblems || 100;
	// Revalidate any open text documents
	documents.all().forEach(validateTextDocument);
});

function splitIntoLines(data: string) {
	return data.split(/\r?\n/g)
}
function validateTextDocument(_: TextDocument): void {
}

connection.onDidChangeWatchedFiles((_change) => {
	// Monitored files have change in VSCode
	_change.changes.forEach(chg => {
		connection.console.log('We received an file change event ' + chg.uri);
		let fspath = Uri.parse(chg.uri).fsPath;
		const ext = path.extname(fspath)
		switch (ext) {
			case '.tconf':
				const confLines = splitIntoLines(fs.readFileSync(fspath).toString())
				easyTypesParser.parseConfiguration(confLines)
				break
			// case '.ty':


		}
	});
});


// This handler provides the initial list of the completion items.
connection.onCompletion((_textDocumentPosition: TextDocumentPositionParams): CompletionItem[] => {

	const carretPos = _textDocumentPosition.position
	let fspath = Uri.parse(_textDocumentPosition.textDocument.uri).fsPath;
	const ext = path.extname(fspath)
	switch (ext) {
		case '.ty':
			if (easyTypesParser.getSuggestions(fspath, carretPos.character, carretPos.line + 1)) {
				const suggestions = typeEngine.getCompletionSuggestions()
				return suggestions
			}
			return []
		default:
			return []
	}

});

// This handler resolve additional information for the item selected in
// the completion list.
connection.onCompletionResolve((item: CompletionItem): CompletionItem => {
	if (item.data === 1) {
		item.detail = 'TypeScript details',
			item.documentation = 'TypeScript documentation'
	} else if (item.data === 2) {
		item.detail = 'JavaScript details',
			item.documentation = 'JavaScript documentation'
	}
	return item;
});

/*
connection.onDidOpenTextDocument((params) => {
	// A text document got opened in VSCode.
	// params.uri uniquely identifies the document. For documents store on disk this is a file URI.
	// params.text the initial full content of the document.
	connection.console.log(`${params.textDocument.uri} opened.`);
});
connection.onDidChangeTextDocument((params) => {
	// The content of a text document did change in VSCode.
	// params.uri uniquely identifies the document.
	// params.contentChanges describe the content changes to the document.
	connection.console.log(`${params.textDocument.uri} changed: ${JSON.stringify(params.contentChanges)}`);
});
connection.onDidCloseTextDocument((params) => {
	// A text document got closed in VSCode.
	// params.uri uniquely identifies the document.
	connection.console.log(`${params.textDocument.uri} closed.`);
});
*/

// Listen on the connection
connection.listen();
