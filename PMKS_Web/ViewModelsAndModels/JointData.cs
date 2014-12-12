using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Silverlight_PMKS;

namespace PMKS_Silverlight_App
{
    public class JointData : DependencyObject, INotifyPropertyChanged
    {

        public double _xPos = double.NaN;
        public double _yPos = double.NaN;
        public double _angle = double.NaN;
        private string _jointType;
        private string _linkNames;

        public string JointType
        {
            get { return _jointType; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) _jointType = "";
                else _jointType = value.Split(',', ' ')[0];
            }
        }

        public string LinkNames
        {
            get { return _linkNames; }
            set
            {
                _linkNames = value;
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("gnd"))
                    _linkNames = _linkNames.Replace("gnd", "ground");
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("grnd"))
                    _linkNames = _linkNames.Replace("grnd", "ground");
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("grond"))
                    _linkNames = _linkNames.Replace("grond", "ground");
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("gound"))
                    _linkNames = _linkNames.Replace("gound", "ground");
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("groud"))
                    _linkNames = _linkNames.Replace("groud", "ground");
                if (_linkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("0"))
                    _linkNames = _linkNames.Replace("0", "ground");

                if (Application.Current.RootVisual != null)
                    App.main.linkInputTable.UpdateLinksTableAfterAdd(this);
            }
        }

        public string XPos
        {
            get { return (double.IsNaN(_xPos)) ? "" : _xPos.ToString("F3", CultureInfo.InvariantCulture); }
            set
            {
                if (!double.TryParse(value, out _xPos))
                    _xPos = double.NaN;
            }
        }

        public string YPos
        {
            get { return (double.IsNaN(_yPos)) ? "" : _yPos.ToString("F3", CultureInfo.InvariantCulture); }
            set
            {
                if (!double.TryParse(value, out _yPos))
                    _yPos = double.NaN;
            }
        }
        // Declare the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        private void onPropertyChanged(object sender, string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string[] LinkNamesList
        {
            get
            {
                if (LinkNames != null)
                    return LinkNames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                else return new string[0];
            }
        }


        public string Angle
        {
            get
            {
                if (double.IsNaN(_angle)
                    && (string.Equals(JointType, "R", StringComparison.InvariantCultureIgnoreCase)
                    || string.IsNullOrWhiteSpace(JointType)))
                    return "";
                return _angle.ToString("F3", CultureInfo.InvariantCulture);
            }
            set
            {
                if (!double.TryParse(value, out _angle))
                    _angle = 0.0;
            }
        }

        //get
        //{
        //    if (double.IsNaN(_angle))
        //    {
        //        if (string.Equals(JointType, "P", StringComparison.InvariantCultureIgnoreCase) ||
        //            string.Equals(JointType, "RP", StringComparison.InvariantCultureIgnoreCase))
        //            return "REQUIRED";
        //        else return "";
        //    }
        //    if (App.main.AngleUnits == AngleType.Radians)
        //        return _angle.ToString(CultureInfo.InvariantCulture);
        //    else
        //        return (DisplayConstants.RadiansToDegrees * _angle).ToString(CultureInfo.InvariantCulture);
        //}
        //set
        //{
        //    if (!double.TryParse(value, out _angle))
        //        _angle = double.NaN;
        //    if (App.main.AngleUnits == AngleType.Degrees)
        //        _angle /= DisplayConstants.RadiansToDegrees;
        //    while (_angle > Math.PI / 2) _angle -= Math.PI;
        //    while (_angle < -Math.PI / 2) _angle += Math.PI;
        //}
        //}

        public Boolean PosVisible
        {
            get { return (Boolean)GetValue(PosVisibleProperty); }
            set { SetValue(PosVisibleProperty, value); }
        }

        public static readonly DependencyProperty PosVisibleProperty
            = DependencyProperty.Register("PosVisible",
                                          typeof(Boolean), typeof(JointData),
                                          new PropertyMetadata(true));

        public Boolean VelocityVisible
        {
            get { return (Boolean)GetValue(VelVisibleProperty); }
            set { SetValue(VelVisibleProperty, value); }
        }

        public static readonly DependencyProperty VelVisibleProperty = DependencyProperty.Register("VelVisible",
                                          typeof(Boolean), typeof(JointData),
                                          new PropertyMetadata(false));

        public Boolean AccelerationVisible
        {
            get { return (Boolean)GetValue(AccelVisibleProperty); }
            set { SetValue(AccelVisibleProperty, value); }
        }

        public static readonly DependencyProperty AccelVisibleProperty = DependencyProperty.Register("AccelVisible",
                                          typeof(Boolean), typeof(JointData),
                                          new PropertyMetadata(false));
        public Boolean DrivingInput
        {
            get { return (Boolean)GetValue(DrivingInputProperty); }
            set { SetValue(DrivingInputProperty, value); }
        }

        public static readonly DependencyProperty DrivingInputProperty = DependencyProperty.Register("DrivingInput",
                                          typeof(Boolean), typeof(JointData),
                                          new PropertyMetadata(false));


        public Boolean CanBeDriver { get { return (LinkNames != null && LinkNames.Contains("ground")); } }

        internal static bool ConvertTextToData(string text, out List<JointData> jointsInfo)
        {
            jointsInfo = new List<JointData>();
            var pivotSentences = text.Split('\n').ToList();
            pivotSentences.RemoveAll(string.IsNullOrWhiteSpace);
            if (pivotSentences.Count == 0) return false;
            foreach (var pivotSentence in pivotSentences)
            {
                var words = pivotSentence.Split(',', ' ', '|').ToList();
                words.RemoveAll(string.IsNullOrWhiteSpace);
                var lastJointType = words.LastOrDefault(s => s.Equals("R", StringComparison.InvariantCultureIgnoreCase)
                                                             ||
                                                             s.Equals("P", StringComparison.InvariantCultureIgnoreCase)
                                                             ||
                                                             s.Equals("RP", StringComparison.InvariantCultureIgnoreCase)
                                                             ||
                                                             s.Equals("G", StringComparison.InvariantCultureIgnoreCase));
                var jointTypeIndex = words.LastIndexOf(lastJointType);
                var index = jointTypeIndex;
                if (index <= 0) return false;
                var typeString = words[index];
                string angleTemp = "";
                double temp;
                if (words.Count() < index + 3) return false;
                if (!double.TryParse(words[index + 1], out temp) || !double.TryParse(words[index + 2], out temp))
                    return false;
                string Xtemp = words[++index];
                string Ytemp = words[++index];
                if ((words.Count() > ++index) && double.TryParse(words[index], out temp))
                {
                    angleTemp = words[index];
                    index++;
                }
                var bools = new bool[4];
                if (words.Count() > index && (words[index].Contains("t") || words[index].Contains("f")))
                {
                    var plusMinusString = words[index];
                    int i = 0;
                    while (i < plusMinusString.Length)
                    {
                        if (plusMinusString[i].Equals('t')) bools[i] = true;
                        i++;
                    }
                }
                else
                {
                    int i = 0;
                    while (index + i < words.Count)
                    {
                        Boolean.TryParse(words[index], out bools[i]);
                        i++;
                    }
                }
                words.RemoveRange(jointTypeIndex, words.Count - jointTypeIndex);
                var linkIDStr = words.Aggregate("", (current, s) => current + ("," + s));
                linkIDStr = linkIDStr.Remove(0, 1);
                jointsInfo.Add(new JointData
                    {
                        JointType = typeString,
                        XPos = Xtemp,
                        YPos = Ytemp,
                        Angle = angleTemp,
                        LinkNames = linkIDStr,
                        PosVisible = bools[0],
                        VelocityVisible = bools[1],
                        AccelerationVisible = bools[2],
                        DrivingInput = bools[3]
                    });
            }
            jointsInfo.Add(new JointData());
            if (jointsInfo.All(jd => !jd.DrivingInput))
                jointsInfo.First(jd => jd.CanBeDriver).DrivingInput = true;
            return true;
        }

        internal static string ConvertDataToText(ObservableCollection<JointData> collection)
        {
            var text = "";
            foreach (var jInfo in collection)
            {
                if (string.IsNullOrWhiteSpace(jInfo.LinkNames) || (string.IsNullOrWhiteSpace(jInfo.JointType)))
                    continue;
                text += jInfo.LinkNames + ",";
                text += jInfo.JointType + ",";
                text += jInfo.XPos + ",";
                text += jInfo.YPos;
                text += (!string.IsNullOrWhiteSpace(jInfo.Angle)) ? "," + jInfo.Angle : "";
                var boolStr = ","
                              + (jInfo.PosVisible ? 't' : 'f')
                              + (jInfo.VelocityVisible ? 't' : 'f')
                              + (jInfo.AccelerationVisible ? 't' : 'f')
                              + (jInfo.DrivingInput ? 't' : 'f');
                //var boolStr = "," + jInfo.PosVisible
                //    + "," + jInfo.VelocityVisible
                //    + "," + jInfo.AccelerationVisible
                //    + "," + jInfo.DrivingInput;
                //while (boolStr.EndsWith(",False"))
                //{
                //    boolStr = boolStr.Remove(boolStr.Length - 6);
                //}
                text += boolStr + "\n";
            }
            return text;
        }


        public void RefreshTablePositions()
        {
            //if (App.main != null)
            //    App.main.fileAndEditPanel.dataGrid.InvalidateMeasure();      
            onPropertyChanged(this, "XPos");
            onPropertyChanged(this, "YPos");
            onPropertyChanged(this, "Angle");
        }
    }
}

