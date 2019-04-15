namespace SudokuLib

open System.Xml.Serialization
    

[<CLIMutable>] 
[<XmlType("header")>]
type Header = {

    [<XmlAttribute>]
    version : int
}


[<CLIMutable>] 
[<XmlType("col")>]
type Col = {

    [<XmlAttribute>]
    index : int

    [<XmlAttribute>]
    readOnly : bool
       
    [<XmlText>]
    data : int
}


[<CLIMutable>] 
[<XmlType("row")>]
type Row = {
    [<XmlAttribute>]
    index : int

    [<XmlElement("cell", IsNullable=true)>]
    Col : Col[]
}



[<CLIMutable>] 
[<XmlRoot(Namespace="", ElementName="Puzzle")>]
type PuzzleXml = {
    [<XmlAttribute>]
    size : int
    header : Header

    [<XmlArray("board")>]
    [<XmlArrayItem("row", typeof<Row>, IsNullable=false)>]
    board  : Row[]
}


module FileTypes = 

    [<Literal>]
    let xmlSample = """
        <Puzzle size="9">
            <header version="1" />
            <board>
            <row index="0">
                <cell index="0" readOnly="true">7</cell>

                <cell index="1" readOnly="true">4</cell>
                <cell index="4" readOnly="true">3</cell>
                <cell index="6" readOnly="true">8</cell>
            </row>
            <row index="1">
                <cell index="2" readOnly="true">8</cell>
                <cell index="4" readOnly="true">1</cell>
                <cell index="5" readOnly="true">2</cell>
                <cell index="8" readOnly="true">6</cell>
            </row>
            <row index="2">
                <cell index="7" readOnly="true">5</cell>
            </row>
            </board>
        </Puzzle>"""


    [<Literal>]
    let xmlDefault = """
        <Puzzle size="9">
          <header version="1" />
          <board>
            <row index="0">
              <cell index="0" readOnly="true">7</cell>
              <cell index="1" readOnly="true">4</cell>
              <cell index="4" readOnly="true">3</cell>
              <cell index="6" readOnly="true">8</cell>
            </row>
            <row index="1">
              <cell index="2" readOnly="true">8</cell>
              <cell index="4" readOnly="true">1</cell>
              <cell index="5" readOnly="true">2</cell>
              <cell index="8" readOnly="true">6</cell>
            </row>
            <row index="2">
              <cell index="7" readOnly="true">5</cell>
            </row>
            <row index="3">
              <cell index="1" readOnly="true">2</cell>
              <cell index="2" readOnly="true">4</cell>
              <cell index="7" readOnly="true">6</cell>
            </row>
            <row index="4">
              <cell index="0" readOnly="true">5</cell>
              <cell index="2" readOnly="true">9</cell>
              <cell index="7" readOnly="true">2</cell>
              <cell index="8" readOnly="true">4</cell>
            </row>
            <row index="5">
              <cell index="2" readOnly="true">3</cell>
              <cell index="3" readOnly="true">5</cell>
              <cell index="6" readOnly="true">1</cell>
            </row>
            <row index="6">
              <cell index="7" readOnly="true">9</cell>
            </row>
            <row index="7">
              <cell index="0" readOnly="true">6</cell>
              <cell index="1" readOnly="true">1</cell>
              <cell index="5" readOnly="true">8</cell>
            </row>
            <row index="8">
              <cell index="3" readOnly="true">2</cell>
              <cell index="5" readOnly="true">7</cell>
            </row>
          </board>
        </Puzzle>"""
