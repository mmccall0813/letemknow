﻿using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace macrottie.letemknow.Patches;

public class TitlePatcher : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player_label.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var titleUpdateWaiter = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "_update_title" },
            t => t.Type is TokenType.ParenthesisOpen,
            t => t.Type is TokenType.ParenthesisClose,
            t => t.Type is TokenType.Colon,
            t => t.Type is TokenType.Newline
        ]);
        var beginningWaiter = new MultiTokenWaiter([
            t => t.Type is TokenType.PrExtends,
            t => t.Type is TokenType.Identifier
        ]);

        foreach (var token in tokens)
        {
            if (beginningWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.Newline);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("current_song");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.Newline);
            } else if (titleUpdateWaiter.Check(token))
            {
                yield return token;

                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("player_id");
                yield return new Token(TokenType.OpIn);
                yield return new IdentifierToken("Network");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.Colon);
                yield return new Token(TokenType.Newline, 2);

                yield return new IdentifierToken("title");

                yield return new Token(TokenType.OpAssign);
                
                // janky, but avoids a Lure conflict.
                yield return new Token(TokenType.Self);
                yield return new Token(TokenType.BracketOpen);
                yield return new ConstantToken(new StringVariant("title"));
                yield return new Token(TokenType.BracketClose);
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("replace");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new IdentifierToken("current_song");
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new StringVariant(""));
                yield return new Token(TokenType.ParenthesisClose);
                
                yield return new Token(TokenType.Newline, 2);
                yield return new IdentifierToken("current_song");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("Network");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("ID_SONG_MAP");
                yield return new Token(TokenType.BracketOpen);
                yield return new IdentifierToken("player_id");
                yield return new Token(TokenType.BracketClose);
                
                yield return new Token(TokenType.Newline, 2);
                yield return new IdentifierToken("title");
                yield return new Token(TokenType.OpAssignAdd);
                yield return new IdentifierToken("current_song");


                yield return new Token(TokenType.Newline, 1);
            }
            else
            {
                yield return token;
            }
        }
    }
}