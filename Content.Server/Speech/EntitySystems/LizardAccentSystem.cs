﻿using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.Random;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class LizardAccentSystem : EntitySystem
{
    private static readonly Regex RegexLowerS = new("s+");
    private static readonly Regex RegexUpperS = new("S+");
    private static readonly Regex RegexInternalX = new(@"(\w)x");
    private static readonly Regex RegexLowerEndX = new(@"\bx([\-|r|R]|\b)");
    private static readonly Regex RegexUpperEndX = new(@"\bX([\-|r|R]|\b)");

    [Dependency] private readonly IRobustRandom _random = default!; // Corvax-Localization

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // hissss
        message = RegexLowerS.Replace(message, "sss");
        // hiSSS
        message = RegexUpperS.Replace(message, "SSS");
        // ekssit
        message = RegexInternalX.Replace(message, "$1kss");
        // ecks
        message = RegexLowerEndX.Replace(message, "ecks$1");
        // eckS
        message = RegexUpperEndX.Replace(message, "ECKS$1");

        // Corvax-Localization-Start
        // c => ссс
        message = Regex.Replace(
            message,
            "с+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // С => CCC
        message = Regex.Replace(
            message,
            "С+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // з => ссс
        message = Regex.Replace(
            message,
            "з+",
            _random.Pick(new List<string>() { "сс", "ссс" })
        );
        // З => CCC
        message = Regex.Replace(
            message,
            "З+",
            _random.Pick(new List<string>() { "СС", "ССС" })
        );
        // ш => шшш,щщщ
        message = Regex.Replace(
            message,
            "ш+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" }) // Adventure
        );
        // Ш => ШШШ,ЩЩЩ
        message = Regex.Replace(
            message,
            "Ш+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" }) // Adventure
        );
        // ч => щщщ,шшш
        message = Regex.Replace(
            message,
            "ч+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" }) // Adventure
        );
        // Ч => ЩЩЩ,ШШШ
        message = Regex.Replace(
            message,
            "Ч+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" }) // Adventure
        );
        // Corvax-Localization-End

        // Adventure-Localization-Start
        // ж => жжж
        message = Regex.Replace(
            message,
            "ж+",
            _random.Pick(new List<string>() { "шш", "шшш", "щщ", "щщщ" })
        );
        // Ж => ЖЖЖ
        message = Regex.Replace(
            message,
            "Ж+",
            _random.Pick(new List<string>() { "ШШ", "ШШШ", "ЩЩ", "ЩЩЩ" })
        );
        // щ => щщщ,шшш
        message = Regex.Replace(
            message,
            "щ+",
            _random.Pick(new List<string>() { "щщ", "щщщ", "шш", "шшш" })
        );
        // Щ => ЩЩЩ,ШШШ
        message = Regex.Replace(
            message,
            "Щ+",
            _random.Pick(new List<string>() { "ЩЩ", "ЩЩЩ", "ШШ", "ШШШ" })
        );
        // ц => ссс
        message = Regex.Replace(
            message,
            "ц+",
            _random.Pick(new List<string> { "сс", "ссс" })
        );
        // Ц => ССС
        message = Regex.Replace(
            message,
            "Ц+",
            _random.Pick(new List<string> { "СС", "ССС" })
        );
        // Adventure-Localization-End

        args.Message = message;
    }
}

