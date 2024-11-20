using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace macrottie.letemknow.Patches;

public class TitlePatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player_label.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var titleUpdateWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "_name" },
            t => t.Type is TokenType.OpAssign,
            t => t is IdentifierToken { Name: "_name" },
            t => t.Type is TokenType.Period,
            t => t.Type is TokenType.Identifier,
            t => t.Type is TokenType.ParenthesisOpen,
            t => t.Type is TokenType.Constant,
            t => t.Type is TokenType.Comma,
            t => t.Type is TokenType.Constant,
            t => t.Type is TokenType.ParenthesisClose,
            t => t.Type is TokenType.Newline
        ]);

        foreach (var token in tokens)
        {
            if (titleUpdateWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("player_id");
                yield return new Token(TokenType.OpIn);
                yield return new IdentifierToken("Network");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.Colon);

                yield return new IdentifierToken("title");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.OpAdd);
                yield return new IdentifierToken("Network");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.BracketOpen);
                yield return new IdentifierToken("player_id");
                yield return new Token(TokenType.BracketClose);

                yield return new Token(TokenType.Newline, 1);
            }
            else
            {
                yield return token;
            }
        }
    }
}