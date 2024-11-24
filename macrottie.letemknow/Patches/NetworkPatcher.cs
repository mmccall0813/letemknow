using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace macrottie.letemknow.Patches;

public class NetworkPatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Singletons/SteamNetwork.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var packetParserWaiter = new MultiTokenWaiter([
            t => t is Token { Type: TokenType.Newline, AssociatedData: 2},
            t => t.Type is TokenType.CfMatch,
            t => t is IdentifierToken { Name: "type" },
            t => t.Type is TokenType.Colon
        ]);
        var beginningWaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrExtends,
            t => t.Type is TokenType.Identifier
        ]);
        
        foreach(var token in tokens)
        {
            if (beginningWaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.OpAssign);
                yield return new Token(TokenType.CurlyBracketOpen);
                yield return new Token(TokenType.CurlyBracketClose);
            }
            else if (packetParserWaiter.Check(token))
            {
                yield return token;
                yield return new Token(TokenType.Newline, 3);
                yield return new ConstantToken(new StringVariant("update_song"));
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.Newline, 4);
                
                yield return new Token(TokenType.BuiltInFunc, (uint?) BuiltinFunction.TextPrint);
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("RECEIVED SONG UPDATE PACKET FROM "));
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("PACKET_SENDER");
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 4);
                
                yield return new Token(TokenType.CfIf);
                yield return new Token(TokenType.OpNot);
                yield return new IdentifierToken("_validate_packet_information");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("DATA");
                yield return new Token(TokenType.Comma);
                yield return new Token(TokenType.BracketOpen);
                yield return new ConstantToken(new StringVariant("song"));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new StringVariant("artist"));
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.Comma);
                yield return new Token(TokenType.BracketOpen);
                yield return new IdentifierToken("TYPE_STRING");
                yield return new Token(TokenType.Comma);
                yield return new IdentifierToken("TYPE_STRING");
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.CfReturn);
                yield return new Token(TokenType.Newline, 4);
                
                yield return new Token(TokenType.CfFor);
                yield return new IdentifierToken("actor");
                yield return new Token(TokenType.OpIn);
                yield return new IdentifierToken("get_tree");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("get_nodes_in_group");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("actor"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.Newline, 5);
                
                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("actor");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("actor_type");
                yield return new Token(TokenType.OpEqual);
                yield return new ConstantToken(new StringVariant("player"));
                yield return new Token(TokenType.OpAnd);
                yield return new IdentifierToken("actor");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("owner_id");
                yield return new Token(TokenType.OpEqual);
                yield return new IdentifierToken("PACKET_SENDER");
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.Newline, 6);

                // ID_SONG_MAP[PACKET_SENDER] = ""
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.BracketOpen);
                yield return new IdentifierToken("PACKET_SENDER");
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.Newline, 6);
                
                // if DATA.song != "": 
                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("DATA");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpNotEqual);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.Colon);
                
                // ID_SONG_MAP[PACKET_SENDER] = "\n" + DATA.song + " - " + DATA.artist
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.BracketOpen);
                yield return new IdentifierToken("PACKET_SENDER");
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant("\n"));
                yield return new Token(TokenType.OpAdd);
                yield return new IdentifierToken("DATA");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("song");
                yield return new Token(TokenType.OpAdd);
                yield return new ConstantToken(new StringVariant(" - "));
                yield return new Token(TokenType.OpAdd);
                yield return new IdentifierToken("DATA");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("artist");
                
                yield return new Token(TokenType.Newline, 6);
                yield return new IdentifierToken("actor");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("_update_title");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new Token(TokenType.ParenthesisClose);

                yield return new Token(TokenType.Newline, 3);
            }
            else
            {
                yield return token;
            }
        }
    }
}