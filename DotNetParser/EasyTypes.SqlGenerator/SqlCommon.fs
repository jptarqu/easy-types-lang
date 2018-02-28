namespace EasyTypes.SqlGenerator

module SqlCommon =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes
    open PoorMansTSqlFormatterLib.Formatters
    open PoorMansTSqlFormatterLib.Tokenizers
    open PoorMansTSqlFormatterLib.Parsers

    let convertToSqlType dataReqs =
        match dataReqs with
        | CommonDataRequirementsString dreq -> 
            let size = if dreq.Size = System.Int32.MaxValue then "max" else dreq.Size.ToString()
            "varchar(" +  size + ") NOT NULL"
        | CommonDataRequirementsStringPattern dreq -> 
            let size = if dreq.Size = System.Int32.MaxValue then "max" else dreq.Size.ToString()
            "varchar(" +  size + ") NOT NULL"
        | CommonDataRequirementsStringChoices dreq -> 
            "varchar(" +  dreq.Size.ToString() + ") NOT NULL"
        | CommonDataRequirementsInt _ -> "int NOT NULL"
        | CommonDataRequirementsDecimal dreq -> "numeric(" +  dreq.Size.ToString() + ", " + dreq.Precision.ToString() + ") NOT NULL"
        | CommonDataRequirementsDate dreq -> "Date" + if dreq.Optional then " NULL" else " NOT NULL"
        | CommonDataRequirementsDateTime dreq -> "DateTime" + if dreq.Optional then " NULL" else " NOT NULL"
        | CommonDataRequirementsMoney _ -> "Money NOT NULL"
        | CommonDataRequirementsBinary _ -> "varbinary(max) NOT NULL"
        | _ -> "varchar(10) NOT NULL"
    let convertToSqlTypeForParam dataReqs =
        match dataReqs with
        | CommonDataRequirementsString dreq -> 
            let size = if dreq.Size = System.Int32.MaxValue then "max" else dreq.Size.ToString()
            "varchar(" +  size + ") "
        | CommonDataRequirementsStringPattern dreq -> 
            let size = if dreq.Size = System.Int32.MaxValue then "max" else dreq.Size.ToString()
            "varchar(" +  size + ") "
        | CommonDataRequirementsStringChoices dreq -> 
            "varchar(" +  dreq.Size.ToString() + ") "
        | CommonDataRequirementsInt _ -> "int"
        | CommonDataRequirementsDecimal dreq -> "numeric(" +  dreq.Size.ToString() + ", " + dreq.Precision.ToString() + ")"
        | CommonDataRequirementsDate dreq -> "Date" + if dreq.Optional then " = NULL" else ""
        | CommonDataRequirementsDateTime dreq -> "DateTime" + if dreq.Optional then " = NULL" else ""
        | CommonDataRequirementsMoney _ -> "Money "
        | CommonDataRequirementsBinary _ -> "varbinary(max) "
        | _ -> "varchar(10) "
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