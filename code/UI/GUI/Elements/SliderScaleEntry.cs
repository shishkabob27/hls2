using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Sandbox.UI
{
    /// <summary>
    /// A horizontal slider with a text entry on the right
    /// </summary>
    public class SliderScaleEntry : Panel
    {
        public SliderScale Slider { get; protected set; }
        public TextEntry TextEntry { get; protected set; }

        public float MinValue
        {
            get => Slider.MinValue;
            set
            {
                Slider.MinValue = value;
                TextEntry.MinValue = value;
            }
        }

        public float MaxValue
        {
            get => Slider.MaxValue;
            set
            {
                Slider.MaxValue = value;
                TextEntry.MaxValue = value;
            }
        }

        public float Step
        {
            get => Slider.Step;
            set => Slider.Step = value;
        }

        public string Format
        {
            get => TextEntry.NumberFormat;
            set => TextEntry.NumberFormat = value;
        }


        public SliderScaleEntry()
        {
            AddClass("sliderentry");

            Slider = AddChild<SliderScale>();
            TextEntry = AddChild<TextEntry>();
            TextEntry.Numeric = true;
            TextEntry.NumberFormat = "0.###";

            TextEntry.Bind("value", Slider, "Value");

            Slider.AddEventListener("value.changed", () => OnValueChanged(Slider.Value));
            TextEntry.AddEventListener("value.changed", () => OnValueChanged(TextEntry.Text));
        }

        protected void OnValueChanged(object value)
        {
            CreateValueEvent("value", value);
        }

        /// <summary>
        /// The actual value. Setting the value will snap and clamp it.
        /// </summary>
        public float Value
        {
            get => Slider.Value;
            set => Slider.Value = value;
        }

        public override void SetProperty(string name, string value)
        {
            if (name == "min" || name == "max" || name == "value" || name == "step" || name == "mintext" || name == "maxtext")
            {
                Slider.SetProperty(name, value);
                return;
            }

            if (name == "format")
            {
                TextEntry.NumberFormat = value;
                return;
            }

            base.SetProperty(name, value);
        }
    }

    namespace Construct
    {
        public static class SliderScaleWithEntryConstructor
        {
            public static SliderScaleEntry SliderScaleWithEntry(this PanelCreator self, float min, float max, float step, string mintext, string maxtext)
            {
                var control = self.panel.AddChild<SliderScaleEntry>();
                control.MinValue = min;
                control.MaxValue = max;
                control.Step = step;

                return control;
            }
        }
    }
}
