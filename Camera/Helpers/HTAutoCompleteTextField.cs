using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.Helpers
{
    public class AutoCompleteTextField : UITextField
    {
        const int AutoCompleteButtonWidth = 30;
        UILabel _autoCompleteLabel;
        UIButton _autoCompleteButton;
        string _autoCompleteString;
        bool _ignoreCase;
        
        bool _showAutoCompleteButton;

        public AutoCompleteTextField(RectangleF frame)
            : base(frame)
        {
            SetupAutocompleteTextField();
        }

        public AutoCompleteTextField() : base()
        {
            SetupAutocompleteTextField();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetupAutocompleteTextField();
        }
        protected override void Dispose(bool disposing)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this,TextFieldTextDidChangeNotification,this);
        }
        void SetupAutocompleteTextField()
        {
            _autoCompleteLabel = new UILabel(RectangleF.Empty) {
                Font = Font,
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.LightGray,
                LineBreakMode = UILineBreakMode.Clip,
                Hidden = true
            };
            AddSubview(_autoCompleteLabel);
            BringSubviewToFront(_autoCompleteLabel);

            _autoCompleteButton = new UIButton(UIButtonType.Custom);
            _autoCompleteButton.TouchUpInside += AutoCompleteText;
            _autoCompleteButton.SetImage(new UIImage(),UIControlState.Normal);
            AddSubview(_autoCompleteButton);
            BringSubviewToFront(_autoCompleteButton);
            AutoCompleteString = "";
            _ignoreCase = true;
            NSNotificationCenter.DefaultCenter.AddObserver(TextFieldTextDidChangeNotification, TextDidChange);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _autoCompleteButton.Frame = FrameForAutocompleteButton;
        }

        public PointF AutoCompleteTextOffset { get; set; }
        public IAutoCompleteDataSource AutoCompleteDataSource { get; set; }
        public bool AutoCompleteDisabled { get; set; }

        public override UIFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                _autoCompleteLabel.Font = value;
            }
        }
        public override bool BecomeFirstResponder()
        {
            BringSubviewToFront(_autoCompleteButton);
            if (!AutoCompleteDisabled)
            {
                if (ClearsOnBeginEditing)
                {
                    _autoCompleteLabel.Text = "";
                }
                _autoCompleteLabel.Hidden = false;
            }
            return base.BecomeFirstResponder();
        }
        public override bool ResignFirstResponder()
        {
            if (!AutoCompleteDisabled)
            {
                _autoCompleteLabel.Hidden = true;
                if (CommitAutocompleteText())
                {
                    NSNotificationCenter.DefaultCenter.PostNotificationName(TextFieldTextDidChangeNotification,this);
                }
            }
            return base.ResignFirstResponder();
        }

        RectangleF AutoCompleteRectForBounds()
        {
            var textRect = TextRect(Bounds);
            var prefixTextSize = new NSString(Text).StringSize(Font, textRect.Size, UILineBreakMode.CharacterWrap);
            var autoCompleteTextSize = new NSString(_autoCompleteString).StringSize(Font,
                                                                                    new SizeF(
                                                                                        textRect.Width -
                                                                                        prefixTextSize.Width,
                                                                                        textRect.Height),
                                                                                    UILineBreakMode.CharacterWrap);
            return new RectangleF(textRect.X+prefixTextSize.Width+AutoCompleteTextOffset.X,
                textRect.Y + AutoCompleteTextOffset.Y,
                autoCompleteTextSize.Width,
                textRect.Height);
        }

        void TextDidChange(NSNotification obj)
        {
            RefreshAutoCompleteText();
        }
        void UpdateAutoCompleteLabel()
        {
            _autoCompleteLabel.Text = AutoCompleteString;
            _autoCompleteLabel.SizeToFit();
            _autoCompleteLabel.Frame = AutoCompleteRectForBounds();
        }
        void RefreshAutoCompleteText()
        {
            if (!AutoCompleteDisabled)
            {
                if (AutoCompleteDataSource!=null)
                {
                    AutoCompleteDataSource.PrepareAutoCompleteText(this, Text, _ignoreCase,(autoCompleteString) =>
                        {
                            AutoCompleteString = autoCompleteString;
                            if (_autoCompleteString.Length > 0)
                            {
                                if (Text.Length == 0 || Text.Length == 1)
                                {
                                    UpdateAutoCompleteButton(true);
                                }
                            }
                            UpdateAutoCompleteLabel();        
                        });
                    
                }
            }
        }

        bool CommitAutocompleteText()
        {
            
            var currentText = Text??"";
            if ((!string.IsNullOrEmpty(AutoCompleteString)) && (!AutoCompleteDisabled))
            {
                Text = String.Format("{0}{1}", Text, _autoCompleteString);
                _autoCompleteString = "";
                UpdateAutoCompleteLabel();
            }

            return !currentText.Equals(Text, StringComparison.InvariantCultureIgnoreCase);
        }
        public void ForceRefreshAutocompleteText()
        {
            RefreshAutoCompleteText();
        }
        string AutoCompleteString
        {
            set {
                _autoCompleteString = value;
                UpdateAutoCompleteButton(true);
            }
            get { return _autoCompleteString; }
        }

        public bool ShowAutoCompleteButton 
        {
            get { return _showAutoCompleteButton; }
            set { _showAutoCompleteButton = value;
                UpdateAutoCompleteButton(true);
            }
        }

        protected RectangleF FrameForAutocompleteButton
        {

            get
            {
                RectangleF buttonRect;
                if(ClearButtonMode==UITextFieldViewMode.Never||Text.Length==0)
                {
                    buttonRect = new RectangleF(Bounds.Width-AutoCompleteButtonWidth,(Bounds.Height/2)-(Bounds.Height-8)/2,AutoCompleteButtonWidth,Bounds.Height-8);
                        
                }
                else
                {
                    buttonRect = new RectangleF(Bounds.Width - 25 - AutoCompleteButtonWidth, (Bounds.Height / 2) - (Bounds.Height - 8) / 2, AutoCompleteButtonWidth, Bounds.Height - 8); 
                }
                return buttonRect;
            }
            
        }

       

        void AutoCompleteText(object sender, EventArgs e)
        {
            if (!AutoCompleteDisabled)
            {
                _autoCompleteLabel.Hidden = false;
                CommitAutocompleteText();
                NSNotificationCenter.DefaultCenter.PostNotificationName(TextFieldTextDidChangeNotification,this);
            }
        }
        void UpdateAutoCompleteButton(bool animated)
        {
            NSAction action = () =>
                {
                    if (_autoCompleteString.Length > 0 && _showAutoCompleteButton)
                    {
                        _autoCompleteButton.Alpha = 1;
                        _autoCompleteButton.Frame = FrameForAutocompleteButton;
                    }
                    else
                    {
                        _autoCompleteButton.Alpha = 0;
                    }
                };
            if (animated)
            {
                
                Animate(0.15,action);
            }
            else
            {
                action.Invoke();   
            }
        }
    }

    public interface IAutoCompleteDataSource
    {
        
        void PrepareAutoCompleteText(AutoCompleteTextField autoCompleteTextField, string text, bool ignoreCase, Action<string> onLookedUp);
    }
}