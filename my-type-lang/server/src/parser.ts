import { PositionInfo, getIndexAfterToken } from './common';
import { ParsePrimtivesFileInfo, parsePrimtivesLines } from './primitiveFileParser';

export interface Prop {
    readonly name: string,
    readonly propTypeName: string,
}
export interface CustomType {
    readonly name: string,
    readonly props: Prop[]
}


export interface TypeNameElement {
    typeName: string,
    position: PositionInfo
}

export interface TypeNameLine {
    readonly kind: 'TypeNameLine',
    readonly lineNum: number,
    readonly nameElement?: TypeNameElement
}

export interface PropNameElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface PropTypeElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface PropLine {
    readonly kind: 'PropLine',
    readonly lineNum: number,
    readonly nameElement: PropNameElement,
    readonly propTypeElement?: PropTypeElement
}
export type SyntaxLineType = PropLine | TypeNameLine

export const typeKeyword = 'type'
function buildTypeNameLine(line: string, lineNum: number): TypeNameLine {
    const startPos = line.lastIndexOf(' ') + 1
    const typeName = line.substring(startPos)
    const endPos = line.length - 1
    return {
        kind: 'TypeNameLine', lineNum, nameElement: {
            typeName,
            position: { startPos, endPos }
        }
    }
}
function buildPropLine(line: string, lineNum: number, linesParsed: SyntaxLineType[]): void {
    if (line.length > 0) {
        // vscode converts tabs into spaces
        const firstWordIdx = getIndexAfterToken(line, ' ')

        const endPos = line.length - 1
        if (firstWordIdx > 0) {
            const propLine = line.substring(firstWordIdx).trim()
            const nameEndIdx = propLine.indexOf(' ')
            const name = propLine.substring(0, nameEndIdx)
            const nameElement: PropNameElement = {
                name,
                position: { startPos: firstWordIdx, endPos: nameEndIdx }
            }
            const restOfLine = propLine.substring(nameEndIdx)

            let propTypeElement: PropTypeElement
            if (restOfLine.trim() !== '') {
                const nextWordIdx = getIndexAfterToken(restOfLine, ' ')
                const typeName = propLine.substring(nextWordIdx).trim()
                propTypeElement = {
                    name: typeName,
                    position: { startPos: nextWordIdx, endPos }
                }
            }
            linesParsed.push({
                kind: 'PropLine',
                lineNum,
                nameElement,
                propTypeElement
            })
        }
    }
}
export interface ParseFileInfo {
    readonly linesParsed: SyntaxLineType[]
    readonly customTypes: CustomType[]
}
function parseLines(lines: string[]): ParseFileInfo {
    const linesParsed: SyntaxLineType[] = []
    const customTypes: CustomType[] = []
    for (let lineIdx = 1; lineIdx < lines.length; lineIdx++) {
        const line = lines[lineIdx];
        if (line.startsWith(typeKeyword)) {
            linesParsed.push(buildTypeNameLine(line, lineIdx + 1))
        } else {
            buildPropLine(line, lineIdx + 1, linesParsed)
        }
    }
    let currType = null
    for (let lineIdx = 0; lineIdx < linesParsed.length; lineIdx++) {
        const line = linesParsed[lineIdx];
        switch (line.kind) {
            case 'TypeNameLine':
                currType = { name: line.nameElement.typeName || '', props: [] }
                customTypes.push(currType)
                break
            case 'PropLine':
                const newProp = {
                    name: line.nameElement.name || '', propTypeName:
                        line.propTypeElement.name || ''
                }
                currType.props.push(newProp)
                break
            default:
                break
        }
    }
    return {
        linesParsed, customTypes
    }
}

export class ProjectSourcesManager {
    private typeFiles: string[] = []
    constructor() {

    }
    addFile(section: string, file: string) {
        if (section.toUpperCase() === 'TYPE') {
            this.typeFiles.push(file)
        }
    }
    getTypeFiles() {
        return this.typeFiles
    }
}
export const primitivesHeader = 'primitives'
export const typesHeader = 'types'

export interface ParseFileInfoHash {
    [docName: string]: ParseFileInfo
}
export interface ParsePrimitivesFileInfoHash {
    [docName: string]: ParsePrimtivesFileInfo
}
export class TypeParser {
    private currParseFilesInfo: ParseFileInfoHash = {}
    private sourcesManager: ProjectSourcesManager
    private currPrimtivesFilesInfo: ParsePrimitivesFileInfoHash = {}
    parse(fileName: string, lines: string[]) {
        const firstLine = lines[0]
        if (firstLine) {
            switch (firstLine) {
                case primitivesHeader:
                    this.parsePrimtives(fileName, lines)
                    break
                case typesHeader:
                    this.parseTypes(fileName, lines)
                    break
                default:
                    break
            }
        }
    }
    parseTypes(fileName: string, lines: string[]) {
        this.currParseFilesInfo[fileName] = parseLines(lines)
    }
    parsePrimtives(fileName: string, lines: string[]) {
        this.currPrimtivesFilesInfo[fileName] = parsePrimtivesLines(lines)
    }

    getSuggestions(fileName: string, charNum: number, lineNumber: number) {
        const fileTypes = this.currParseFilesInfo[fileName]
        if (fileTypes) {
            return this.isPosATypeForProp(fileTypes, charNum, lineNumber)
        }
        const fileProps = this.currPrimtivesFilesInfo[fileName]
        if (fileProps) {
            return this.isPosABaseTypeForPrimtive(fileProps, charNum, lineNumber)
        }
    }
    private isPosATypeForProp(fileInfo: ParseFileInfo, charNum: number, lineNumber: number) {
        if (fileInfo) {
            const syntaxLine = fileInfo.linesParsed[lineNumber]
            if (!syntaxLine) {
                return false
            }
            return (syntaxLine.nameElement.position.endPos + 1) < charNum
        }
        return false
    }
    private isPosABaseTypeForPrimtive(fileInfo: ParsePrimtivesFileInfo, charNum: number, lineNumber: number) {
        if (fileInfo) {

            const syntaxLine = fileInfo.linesParsed[lineNumber]
            if (!syntaxLine) {
                return false
            }
            return ((syntaxLine.nameElement.position.endPos + 1) < charNum) &&
                (syntaxLine.baseArguments.length === 0 ||
                    ((syntaxLine.baseArguments[0].position.startPos) > charNum)
                )
        }
        return false
    }
    parseConfiguration(lines: string[]) {
        if (this.sourcesManager) {
            delete this.sourcesManager
        }
        this.sourcesManager = new ProjectSourcesManager()
        let lastSection = ''
        for (let lineIdx = 0; lineIdx < lines.length; lineIdx++) {
            const line = lines[lineIdx]
            if (line.startsWith('#')) {
                lastSection = line.substring(1)
            } else {
                this.sourcesManager.addFile(lastSection, line)
            }
        }
    }
}