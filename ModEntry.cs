using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace NoClock
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(DayTimeMoneyBox), nameof(DayTimeMoneyBox.draw), new[] { typeof(Microsoft.Xna.Framework.Graphics.SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(BeforeDayTimeMoneyBoxDraw)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(AfterDayTimeMoneyBoxDraw)),
                finalizer: new HarmonyMethod(typeof(ModEntry), nameof(AfterDayTimeMoneyBoxDraw))
            );

            harmony.Patch(
                original: AccessTools.Method(
                    typeof(Utility),
                    nameof(Utility.drawTextWithShadow),
                    new[]
                    {
                        typeof(SpriteBatch),
                        typeof(StringBuilder),
                        typeof(SpriteFont),
                        typeof(Vector2),
                        typeof(Color),
                        typeof(float),
                        typeof(float),
                        typeof(int),
                        typeof(int),
                        typeof(float),
                        typeof(int)
                    }
                ),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(BeforeDrawTextWithShadow))
            );
        }

        private static bool IsDrawingDayTimeMoneyBox;

        private static void BeforeDayTimeMoneyBoxDraw()
        {
            IsDrawingDayTimeMoneyBox = true;
        }

        private static void AfterDayTimeMoneyBoxDraw()
        {
            IsDrawingDayTimeMoneyBox = false;
        }

        private static bool BeforeDrawTextWithShadow(StringBuilder text)
        {
            if (IsDrawingDayTimeMoneyBox && IsClockText(text))
                return false;

            return true;
        }

        private static bool IsClockText(StringBuilder text)
        {
            string value = text.ToString().Trim().ToLowerInvariant();

            return value.Length >= 4
                && value.Length <= 8
                && char.IsDigit(value[0]);
        }
    }
}
