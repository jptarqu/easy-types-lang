import {
    CompletionItem,
    CompletionItemKind
} from 'vscode-languageserver';

export interface PrimitiveDefinition {
    readonly name: string,
    readonly data: string,
}

export interface PrimitiveHash {
    [name: string]: PrimitiveDefinition
}
export class PrimitivesProvider {
    private primitives: PrimitiveHash = {}
    constructor() {

    }
    addPrimitive(primitiveDef: PrimitiveDefinition) {
        this.primitives[primitiveDef.name] = primitiveDef
    }
    getCompletionSuggestions(): CompletionItem[] {
        const suggestions: CompletionItem[] = []
        for (let key in this.primitives) {
            const prim = this.primitives[key]
            suggestions.push({
                label: prim.name,
                kind: CompletionItemKind.Class,
                data: prim.name
            })
        }
        return suggestions
    }
}

export function setupEnv() {
    const primitives = new PrimitivesProvider()
    primitives.addPrimitive({
        name: 'string',
        data: 'string'
    })
    primitives.addPrimitive({
        name: 'integer',
        data: 'int'
    })
    primitives.addPrimitive({
        name: 'money',
        data: 'money'
    })
    return primitives
}
