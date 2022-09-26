using Sandbox.Html;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.UI
{
	/// <summary>
	/// A UI control which provides multiple options via a dropdown box
	/// </summary>
	[Library( "select" ), Alias( "dropdown" )]
	public class DropDown : PopupButton
	{
		protected IconPanel DropdownIndicator;

		/// <summary>
		/// The options to show on click. You can edit these directly via this property.
		/// </summary>
		public List<Option> Options { get; } = new();

		Option selected;

		object _value;
		int _valueHash;

		/// <summary>
		/// The current string value. This is useful to have if Selected is null.
		/// </summary>
		public object Value 
		{
			get => _value;
			set
			{
				if ( _valueHash == HashCode.Combine( value ) )
					return;

				if ( $"{_value}" == $"{value}" )
					return;

				_valueHash = HashCode.Combine( value );
				_value = value;

				if ( _value != null && Options.Count == 0 )
				{
					PopulateOptionsFromType( _value.GetType() );
				}
				
				Select( _value?.ToString(), false );
			}
		}

		/// <summary>
		/// The currently selected option
		/// </summary>
		public Option Selected
		{
			get => selected;
			set
			{
				if ( selected == value ) return;

				selected = value;

				if ( selected != null )
				{
					Value = $"{selected.Value}";
					Icon = selected.Icon;
					Text = selected.Title;

					CreateEvent( "onchange" );
					CreateValueEvent( "value", selected?.Value );
				}
			}
		}

		public DropDown()
		{
			AddClass( "dropdown" );
			DropdownIndicator = Add.Icon( "expand_more", "dropdown_indicator" );
		}

		public DropDown( Panel parent ) : this()
		{
			Parent = parent;
		}

		public override void SetPropertyObject( string name, object value )
		{
			base.SetPropertyObject( name, value );
		}

		/// <summary>
		/// Given the type, populate options. This is useful if you're an enum type.
		/// </summary>
		private void PopulateOptionsFromType( Type type )
		{
			if ( type == typeof( bool ) )
			{
				Options.Add( new Option( "True", true ) );
				Options.Add( new Option( "False", false ) );
				return;
			}

			if ( type.IsEnum )
			{
				var names = type.GetEnumNames();
				var values = type.GetEnumValues();

				for ( int i = 0; i < names.Length; i++ )
				{
					Options.Add( new Option( names[i], values.GetValue( i ) ) );
				}

				return;
			}

			Log.Info( $"Dropdown Type: {type}" );
		}

		/// <summary>
		/// Open the dropdown
		/// </summary>
		public override void Open()
		{
			Popup = new Popup( this, Popup.PositionMode.BelowStretch, 0.0f );
			Popup.AddClass( "flat-top" );

			foreach ( var option in Options )
			{
				var o = Popup.AddOption( option.Title, option.Icon, () => Select( option ) );
				if ( Selected != null && option.Value == Selected.Value )
				{
					o.AddClass( "active" );
				}
			}
		}

		/// <summary>
		/// Select an option
		/// </summary>
		protected virtual void Select( Option option, bool triggerChange = true )
		{
			if ( !triggerChange )
			{
				selected = option;

				if ( option != null )
				{
					Value = option.Value;
					Icon = option.Icon;
					Text = option.Title;
				}
			}
			else
			{
				Selected = option;
			}
		}

		/// <summary>
		/// Select an option by value string
		/// </summary>
		protected virtual void Select( string value, bool triggerChange = true )
		{
			Select( Options.FirstOrDefault( x => string.Equals( x.Value.ToString(), value, StringComparison.OrdinalIgnoreCase ) ), triggerChange );
		}


		/// <summary>
		/// Give support for option elements in html template
		/// </summary>
		public override bool OnTemplateElement( INode element )
		{
			Options.Clear();

			foreach ( var child in element.Children )
			{
				if ( !child.IsElement ) continue;

				//
				// 	<select> <-- this DropDown control
				//		<option value="#f00">Red</option> <-- option
				//		<option value="#ff0">Yellow</option> <-- option
				//		<option value="#0f0">Green</option> <-- option
				// </select>
				//
				if ( child.Name.Equals( "option", StringComparison.OrdinalIgnoreCase ) )
				{
					var o = new Option();

					o.Title = child.InnerHtml;
					o.Value = child.GetAttribute( "value", o.Title );
					o.Icon = child.GetAttribute( "icon", null );

					Options.Add( o );
				}
			}

			Select( $"{Value}" );
			return true;
		}

	}
}
