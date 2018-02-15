namespace EasyTypes.SqlGenerator

module SqlCommon =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes
    open PoorMansTSqlFormatterLib.Formatters
    open PoorMansTSqlFormatterLib.Tokenizers
    open PoorMansTSqlFormatterLib.Parsers

    let convertToSqlType dataReqs =
        match dataReqs with
        | CommonDataRequirementsString dreq -> "varchar(" +  dreq.Size.ToString() + ")"
        | CommonDataRequirementsInt _ -> "int"
        | CommonDataRequirementsDecimal dreq -> "numeric(" +  dreq.Size.ToString() + ", " + dreq.Precision.ToString() + ")"
        | CommonDataRequirementsDate _ -> "Date"
        | CommonDataRequirementsDateTime _ -> "DateTime"
        | CommonDataRequirementsMoney _ -> "Money"
        | CommonDataRequirementsBinary _ -> "varbinary(max)"
        | _ -> "varchar(10)"
    let isPrimaryKey  (p:TypeProperty) = 
        p.propType.name = "IntId"
        
    let GetFormatter(): TSqlStandardFormatter =
        //let options = TSqlStandardFormatterOptions(configString);
        let outFormatter = TSqlStandardFormatter();
        outFormatter
    let private _treeFormatter = GetFormatter()
    let private _tokenizer = TSqlStandardTokenizer();
    let private _parser = TSqlStandardParser();

    let FormatSql (inputSQL:string) : string =
        let tokenized = _tokenizer.TokenizeSQL(inputSQL);
        let parsed = _parser.ParseSQL(tokenized);
        let outputSQL = _treeFormatter.FormatSQLTree(parsed);
        outputSQL