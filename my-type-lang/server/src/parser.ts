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
    for (let lineIdx = 0; lineIdx < lines.length; lineIdx++) {
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
export class TypeParser {
    private currParseFileInfo: ParseFileInfo
    private sourcesManager: ProjectSourcesManager
    private currPrimtivesFileInfo: ParsePrimtivesFileInfo
    parse(lines: string[]) {
        this.currParseFileInfo = parseLines(lines)
    }
    parsePrimtives(lines: string[]) {
        this.currPrimtivesFileInfo = parsePrimtivesLines(lines)
    }
    isPosATypeForProp(charNum: number, lineNumber: number) {
        const syntaxLine = this.currParseFileInfo.linesParsed[lineNumber]
        if (!syntaxLine) {
            return false
        }
        return (syntaxLine.nameElement.position.endPos + 1) < charNum
    }
    isPosABaseTypeForPrimtive(charNum: number, lineNumber: number) {
        const syntaxLine = this.currPrimtivesFileInfo.linesParsed[lineNumber]
        if (!syntaxLine) {
            return false
        }
        return ((syntaxLine.nameElement.position.endPos + 1) < charNum) &&
            (syntaxLine.baseArguments.length === 0 ||
                ((syntaxLine.baseArguments[0].position.startPos) > charNum)
            )
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