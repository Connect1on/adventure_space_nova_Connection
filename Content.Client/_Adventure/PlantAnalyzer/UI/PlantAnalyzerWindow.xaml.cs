using Content.Shared._Adventure.PlantAnalyzer;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using System.Linq;
using System.Text;
using FancyWindow = Content.Client.UserInterface.Controls.FancyWindow;

namespace Content.Client._Adventure.PlantAnalyzer.UI;

[GenerateTypedNameReferences]
public sealed partial class PlantAnalyzerWindow : FancyWindow
{
    private readonly IEntityManager _entityManager;
    private readonly ButtonGroup _buttonGroup = new();

    private const string IndentedNewline = "\n   ";

    public PlantAnalyzerWindow(PlantAnalyzerBoundUserInterface owner)
    {
        RobustXamlLoader.Load(this);

        var dependencies = IoCManager.Instance!;
        _entityManager = dependencies.Resolve<IEntityManager>();

        OnButton.Group = _buttonGroup;
        OnButton.ToggleMode = true;
        OffButton.Group = _buttonGroup;
        OffButton.ToggleMode = true;

        OnButton.OnPressed += _ => owner.AdvPressed(true);
        OffButton.OnPressed += _ => owner.AdvPressed(false);
    }

    public void Populate(PlantAnalyzerScannedSeedPlantInformation msg)
    {
        var target = _entityManager.GetEntity(msg.TargetEntity);
        Title = Loc.GetString("plant-analyzer-interface-title");

        if (target == null)
        {
            NoData.Visible = true;
            return;
        }
        NoData.Visible = false;

        if (msg.AdvancedInfo is not null)
        {
            OnButton.Pressed = true;
        }
        else
        {
            OffButton.Pressed = true;
        }

        // Process message fields into strings.
        StringBuilder chemString = new();
        if (msg.SeedChem != null)
        {
            foreach (var chem in msg.SeedChem)
            {
                chemString.Append(IndentedNewline);
                chemString.Append(chem);
            }
        }

        StringBuilder exudeGases = GetStringFromGasFlags(msg.ExudeGases);
        StringBuilder consudeGases = GetStringFromGasFlags(msg.ConsumeGases);

        if (msg.IsTray)
            PlantName.Text = Loc.GetString("plant-analyzer-window-label-name-scanned-plant", ("seedName", Loc.GetString(string.IsNullOrEmpty(msg.SeedName) ? "plant-analyzer-unknown-plant" : msg.SeedName)));
        else
            PlantName.Text = Loc.GetString("plant-analyzer-window-label-name-scanned-seed", ("seedName", Loc.GetString(string.IsNullOrEmpty(msg.SeedName) ? "plant-analyzer-unknown-plant" : msg.SeedName)));
        // Basics
        PlantYield.Text = Loc.GetString("plant-analyzer-plant-yield-text", ("seedYield", $"{msg.SeedYield:D0}"));
        Potency.Text = Loc.GetString("plant-analyzer-plant-potency-text", ("seedPotency", $"{msg.SeedPotency:F0}"));
        Repeat.Text = Loc.GetString("plant-analyzer-plant-harvest-text", ("plantHarvestType", Loc.GetString($"plant-analyzer-harvest-{msg.HarvestType}").ToString()));
        Endurance.Text = Loc.GetString("plant-analyzer-plant-endurance-text", ("seedEndurance", $"{msg.Endurance:F0}"));
        Chemicals.Text = Loc.GetString("plant-analyzer-plant-chemistry-text", ("seedChem", chemString));
        ExudeGases.Text = Loc.GetString("plant-analyzer-plant-exude-text", ("gases", exudeGases.Length == 0 ? Loc.GetString("plant-analyzer-plant-gases-none") : exudeGases.ToString()));
        ConsumeGases.Text = Loc.GetString("plant-analyzer-plant-consume-text", ("gases", consudeGases.Length == 0 ? Loc.GetString("plant-analyzer-plant-gases-none") : consudeGases.ToString()));
        Lifespan.Text = Loc.GetString("plant-analyzer-plant-lifespan-text", ("lifespan", $"{msg.Lifespan:F1}"));
        Maturation.Text = Loc.GetString("plant-analyzer-plant-maturation-text", ("maturation", $"{msg.Maturation:F1}"));
        Production.Text = Loc.GetString("plant-analyzer-plant-production-text", ("production", $"{msg.Production:F1}"));
        GrowthStages.Text = Loc.GetString("plant-analyzer-plant-growthstages-text", ("growthStages", $"{msg.GrowthStages:D0}"));
        // Tolerances
        var adv = msg.AdvancedInfo;
        NutrientUsage.Text = Loc.GetString("plant-analyzer-tolerance-nutrient-usage", ("nutrientUsage", adv is null ? "-" : $"{adv.Value.NutrientConsumption:F2}"));
        WaterUsage.Text = Loc.GetString("plant-analyzer-tolerance-water-usage", ("waterUsage", adv is null ? "-" : $"{adv.Value.WaterConsumption:F2}"));
        IdealHeat.Text = Loc.GetString("plant-analyzer-tolerance-ideal-heat", ("idealHeat", adv is null ? "-" : $"{adv.Value.IdealHeat:F0}"));
        HeatTolerance.Text = Loc.GetString("plant-analyzer-tolerance-heat-tolerance", ("heatTolerance", adv is null ? "-" : $"{adv.Value.HeatTolerance:F1}"));
        IdealLight.Text = Loc.GetString("plant-analyzer-tolerance-ideal-light", ("idealLight", adv is null ? "-" : $"{adv.Value.IdealLight:F1}"));
        LightTolerance.Text = Loc.GetString("plant-analyzer-tolerance-light-tolerance", ("lightTolerance", adv is null ? "-" : $"{adv.Value.LightTolerance:F1}"));
        ToxinsTolerance.Text = Loc.GetString("plant-analyzer-tolerance-toxin-tolerance", ("toxinsTolerance", adv is null ? "-" : $"{adv.Value.ToxinsTolerance:F1}"));
        LowPressureTolerance.Text = Loc.GetString("plant-analyzer-tolerance-low-pressure", ("lowPressureTolerance", adv is null ? "-" : $"{adv.Value.LowPressureTolerance:F1}")); ;
        HighPressureTolerance.Text = Loc.GetString("plant-analyzer-tolerance-high-pressure", ("highPressureTolerance", adv is null ? "-" : $"{adv.Value.HighPressureTolerance:F1}"));
        PestTolerance.Text = Loc.GetString("plant-analyzer-tolerance-pest-tolerance", ("pestTolerance", adv is null ? "-" : $"{adv.Value.PestTolerance:F1}"));
        WeedTolerance.Text = Loc.GetString("plant-analyzer-tolerance-weed-tolerance", ("weedTolerance", adv is null ? "-" : $"{adv.Value.WeedTolerance:F1}"));
        // Misc

        if (adv != null)
        {
            var advInst = adv.Value;
            StringBuilder mutations = new();
            if (advInst.Mutations.HasFlag(MutationFlags.TurnIntoKudzu))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-turnintokudzu"));
            }
            if (advInst.Mutations.HasFlag(MutationFlags.Seedless))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-seedless"));
            }
            if (advInst.Mutations.HasFlag(MutationFlags.Slip))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-slip"));
            }
            if (advInst.Mutations.HasFlag(MutationFlags.Sentient))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-sentient"));
            }
            if (advInst.Mutations.HasFlag(MutationFlags.Ligneous))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-ligneous"));
            }
            // if (advInst.Mutations.HasFlag(MutationFlags.Bioluminescent))
            // {
            //     mutations.Append(IndentedNewline);
            //     mutations.Append(Loc.GetString("plant-analyzer-mutation-bioluminescent"));
            // }
            if (advInst.Mutations.HasFlag(MutationFlags.CanScream))
            {
                mutations.Append(IndentedNewline);
                mutations.Append(Loc.GetString("plant-analyzer-mutation-canscream"));
            }

            Traits.Text = Loc.GetString("plant-analyzer-plant-mutations-text", ("traits", mutations.ToString()));
        }
        else
        {
            Traits.Text = Loc.GetString("plant-analyzer-plant-mutations-text", ("traits", "-"));
        }

        StringBuilder speciation = new();
        if (msg.Speciation is null)
        {
            speciation.Append("-");
        }
        else
        {
            foreach (var species in msg.Speciation)
            {
                speciation.Append(IndentedNewline);
                speciation.Append(Loc.GetString(species));
            }
        }

        PlantSpeciation.Text = Loc.GetString("plant-analyzer-plant-speciation-text", ("speciation", speciation.ToString()));
    }

    private StringBuilder GetStringFromGasFlags(GasFlags flags)
    {
        StringBuilder output = new();
        if (flags.HasFlag(GasFlags.Nitrogen))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-nitrogen"));
        }
        if (flags.HasFlag(GasFlags.Oxygen))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-oxygen"));
        }
        if (flags.HasFlag(GasFlags.CarbonDioxide))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-co2"));
        }
        if (flags.HasFlag(GasFlags.Plasma))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-plasma"));
        }
        if (flags.HasFlag(GasFlags.Tritium))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-tritium"));
        }
        if (flags.HasFlag(GasFlags.WaterVapor))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-water-vapor"));
        }
        if (flags.HasFlag(GasFlags.Ammonia))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-ammonia"));
        }
        if (flags.HasFlag(GasFlags.NitrousOxide))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-n2o"));
        }
        if (flags.HasFlag(GasFlags.Frezon))
        {
            output.Append(IndentedNewline);
            output.Append(Loc.GetString("gases-frezon"));
        }
        return output;
    }
}
