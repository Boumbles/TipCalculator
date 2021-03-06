﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.FSharp.Core;
using Microsoft.Phone.Controls;


namespace TipCalculator {
    public partial class MainPage : PhoneApplicationPage {
        // Constants
        const String BILL = "Bill : $";
        const String TIP = "Tip : $";
        const String TIPPP = "Tip per person : $";
        const String BILLPP = "Bill per person : $";
        const String TIPSUFFIX = "";
        const double DEFAULTBILL = 40f;
        const double DEFAULTTIPPERC = 15f;
        const bool REVIEW = false;
        // Variables
        bool initialized = false;
        List<List<String>> funphrases;
        List<String> tier1, tier2, tier3, tier4;
        char tempTipValueBoxLastChar = '\0', tempTipPercentBoxLastChar = '\0';

        public MainPage() {
            InitializeComponent();

            var numericScope = new InputScope();
            var numericScopeName = new InputScopeName();
            totalbillblock.Visibility = Visibility.Collapsed;
            totaltipblock.Visibility = Visibility.Collapsed;
            totalperpersonblock.Visibility = Visibility.Collapsed;
            tipperperson.Visibility = Visibility.Collapsed;
            //taxpercentlbl.Visibility = Visibility.Collapsed;
            //taxtextbox.Visibility = Visibility.Collapsed;
            taxtextbox.IsEnabled = false;
            numericScopeName.NameValue = InputScopeNameValue.Number;
            numericScope.Names.Add(numericScopeName);
            billbox.InputScope = numericScope;
            billbox.Text = DEFAULTBILL.ToString("#.##");
            tippercentdsp.Text = DEFAULTTIPPERC.ToString();
            taxtextbox.InputScope = numericScope;
            tipvaluebox.InputScope = numericScope;
            peoplebox.InputScope = numericScope;
            taxtextbox.InputScope = numericScope;
            tippercentdsp.InputScope = numericScope;
            TipPercentBoxChange();

            initialized = true;
            tipreviewdsp.Visibility = REVIEW ? Visibility.Visible : Visibility.Collapsed;
            funphrases = new List<List<String>>();
            PopulateTierLists();
            RecalculateEverything();
        }
        /// <summary>
        /// Fills in the tier lists with all the fun phrases we want to say! :D 
        /// </summary>
        private void PopulateTierLists() {
            //TODO add some more fun phrases!
            String[] tier1arr = { "Seriously?" };
            String[] tier2arr = { "Not so hot eh?" };
            String[] tier3arr = { "You are a good person" };
            String[] tier4arr = { "Marry me!" };

            tier1 = new List<String>(tier1arr);
            tier2 = new List<String>(tier2arr);
            tier3 = new List<String>(tier3arr);
            tier4 = new List<String>(tier4arr);
        }
        private void PickSomethingToSay(int tier) {
            //TODO implement
        }
        /*
         * http://stackoverflow.com/questions/6951335/using-string-format-to-show-decimal-upto-2-places-or-simple-integer
         * */
        private string DoFormat(double myNumber) {
            var s = string.Format("{0:0.00}", myNumber);

          //  if(s.EndsWith("00")) {
         //       return ((int)myNumber).ToString();
         //   } else {
                return s;
          //  }
        }
        private void RecalculateEverything() {
            if(initialized) {
                if(billbox.Text == null || billbox.Text == "") billbox.Text = "0";
                if(taxtextbox.Text == null || taxtextbox.Text == "") taxtextbox.Text = "0";
                if(tipvaluebox.Text == null || tipvaluebox.Text == "") tipvaluebox.Text = "0";
                if(peoplebox.Text == null || peoplebox.Text == "" || peoplebox.Text == "0")
                    peoplebox.Text = "1";
                double bill =
                   System.Convert.ToDouble(billbox.Text[billbox.Text.Length - 1] == '.' ?
                                           billbox.Text + "00" : billbox.Text);
                double tax =
                   System.Convert.ToDouble(taxtextbox.Text[taxtextbox.Text.Length - 1] == '.' ?
                                           taxtextbox.Text + "00" : taxtextbox.Text);
                double tippercent = System.Convert.ToDouble(tip_slider.Value);
                int people = System.Convert.ToInt32(peoplebox.Text);
                double tip = 0;
                if(System.Char.IsDigit(tempTipPercentBoxLastChar)) tempTipPercentBoxLastChar = '\0';
                if(System.Char.IsDigit(tempTipValueBoxLastChar)) tempTipValueBoxLastChar = '\0';
                // if(tippercent > 0) {
                //   if(tax == 0) {
                //      tip = TipCalFS.GetTip(bill, tippercent / 100);
                //   } else {
                tip = TipCalFS.GetTipBeforeTax(bill, tax / 100, tippercent / 100);
                //}
                //  }
                // was using this to put a 0 in front of everything but now we have a function to do that. 
                // keeping the variable here in case we need it for something else
                String total_bill_prefix = "";
                totalbillblock.Text = BILL + total_bill_prefix + DoFormat(bill + tip);
                String total_tip_prefix =  "";
                totaltipblock.Text = TIP + total_tip_prefix + DoFormat(tip);
                totalbillblock.Visibility = Visibility.Visible;
                totaltipblock.Visibility = Visibility.Visible;
                if(people > 1) {
                    String tip_per_person_prefix =  "";
                    tipperperson.Text = TIPPP + tip_per_person_prefix + DoFormat(tip / people);
                    String total_per_person_prefix =  "";
                    totalperpersonblock.Text = BILLPP + total_per_person_prefix +
                        DoFormat((bill + tip) / people);
                    totalperpersonblock.Visibility = Visibility.Visible;
                    tipperperson.Visibility = Visibility.Visible;
                } else {
                    totalperpersonblock.Visibility = Visibility.Collapsed;
                    tipperperson.Visibility = Visibility.Collapsed;
                }
                tipvaluebox.Text = tip.ToString("#.##");// +tempTipValueBoxLastChar;
                tempTipValueBoxLastChar = '\0';
                taxtextbox.Text = tax.ToString();// +tempTipPercentBoxLastChar;
                tempTipPercentBoxLastChar = '\0';
                //billbox.Text = bill.ToString("#.##");
            }
        }
        #region Simple calculations
        /// <summary>
        /// Pass percent as decimal
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private double GetTip(double bill, double percent) {
            return bill * percent;
        }
        /// <summary>
        /// Pass percent and tax as decimal
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="tax"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private double GetTipBeforeTax(double bill, double tax, double percent) {
            return (GetTip(bill / (1 + tax), percent));
        }
        private double GetTipPerPerson(double tip, int people) {
            return tip / people;
        }
        private double GetBillPerPerson(double bill, double tip, int people) {
            return ((bill + tip) / people);
        }
        #endregion

        #region GUI interaction
        private void GUIEventTextChangedCallRecalculateEverything(object sender, TextChangedEventArgs e) {
            //TODO Figure out how to check if we've put a decimal and make sure it has 00s after instead of truncating.

            RecalculateEverything();
        }
        private void GUIEventRoutedEventArgsCallRecalculateEverything(object sender, RoutedEventArgs e) {
            RecalculateEverything();
        }
        private void GUIEventRoutedPropertyChangedEventArgsCallRecalculateEverything(object sender,
           RoutedPropertyChangedEventArgs<double> e) {
            RecalculateEverything();
        }

        private void taxflag_Checked(object sender, RoutedEventArgs e) {
            taxtextbox.IsEnabled = true;
            RecalculateEverything();
        }
        private void taxflag_Unchecked(object sender, RoutedEventArgs e) {
            taxtextbox.IsEnabled = false;
            taxtextbox.Text = "0";
            RecalculateEverything();
        }
        private void plus1_Click(object sender, GestureEventArgs e) {
            peoplebox.Text = (System.Convert.ToInt32(peoplebox.Text) + 1).ToString();
            RecalculateEverything();
        }
        private void minus1_Click(object sender, GestureEventArgs e) {
            int ppl = System.Convert.ToInt32(peoplebox.Text);
            peoplebox.Text = ppl > 1 ? (ppl - 1).ToString() : ppl.ToString();
            RecalculateEverything();
        }
        private void tip_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(tippercentdsp != null) {
                tip_slider.Value = Math.Round(e.NewValue * 2) / 2;
                tippercentdsp.Text = tip_slider.Value.ToString();
            }
            if(tipvaluebox != null) {
                tip_slider.Value = Math.Round(e.NewValue / 0.5 ) * 0.5;
                tipvaluebox.Text = TipCalFS.GetTipBeforeTax(System.Convert.ToDouble(billbox.Text),
                                                            System.Convert.ToDouble(taxtextbox.Text) / 100,
                                                            tip_slider.Value).ToString();
            }
            RecalculateEverything();
        }
        private void tipvaluebox_change_made() {
            double tmptippercent = 0;
            tempTipValueBoxLastChar = tipvaluebox.Text[tipvaluebox.Text.Length - 1];
            double tipValueBoxValue = System.Convert.ToDouble(
                                      tempTipValueBoxLastChar == '.' ?
                                      (tipvaluebox.Text + "00") : tipvaluebox.Text);

            if(taxtextbox != null && billbox != null)
                tmptippercent = TipCalFS.GetTipPercentBeforeTax(tipValueBoxValue,
                                                                System.Convert.ToDouble(taxtextbox.Text) / 100,
                                                                System.Convert.ToDouble(billbox.Text)) * 100;
            if(tip_slider != null)
                //tip_slider.Value = System.Convert.ToDouble(tipvaluebox.Text.Substring(0, tipvaluebox.Text.Length - TIPSUFFIX.Length));
                tip_slider.Value = tmptippercent;
            if(tippercentdsp != null)
                //tippercentdsp.Text = Math.Round(tip_slider.Value, 2).ToString() + TIPSUFFIX;
                tippercentdsp.Text = Math.Round(tip_slider.Value, 2).ToString() + TIPSUFFIX;
            RecalculateEverything();
        }
        private void tipvaluebox_TextChanged(object sender, TextChangedEventArgs e) {
            tipvaluebox_change_made();
        }
        private void tipvaluebox_LostFocus(object sender, RoutedEventArgs e) {
            tipvaluebox_change_made();
        }
        private void TipPercentBoxChange() {
            if(tippercentdsp.Text == null || tippercentdsp.Text == "") tippercentdsp.Text = "0";
            tempTipPercentBoxLastChar = tippercentdsp.Text[tippercentdsp.Text.Length - 1];
            tip_slider.Value = System.Convert.ToDouble(tempTipPercentBoxLastChar == '.' ?
                                                       tippercentdsp.Text += "00" : tippercentdsp.Text);
            tipvaluebox.Text = TipCalFS.GetTipBeforeTax(System.Convert.ToDouble(billbox.Text),
                                                        System.Convert.ToDouble(taxtextbox.Text) / 100,
                                                        tip_slider.Value).ToString();
            RecalculateEverything();
        }
        private void tippercentdsp_TextChanged(object sender, TextChangedEventArgs e) {
            TipPercentBoxChange();
        }
        private void tippercentdsp_LostFocus(object sender, RoutedEventArgs e) {
            TipPercentBoxChange();
        }
        #region Tapping!
        private void billbox_Tap(object sender, GestureEventArgs e) {
            billbox.SelectAll();
        }
        private void taxtextbox_Tap(object sender, GestureEventArgs e) {
            if(taxtextbox.IsEnabled) taxtextbox.SelectAll();
        }

        private void peoplebox_Tap(object sender, GestureEventArgs e) {
            peoplebox.SelectAll();
        }
        private void tipvaluebox_Tap(object sender, GestureEventArgs e) {
            tipvaluebox.SelectAll();
        }
        private void tippercentdsp_Tap(object sender, GestureEventArgs e) {
            tippercentdsp.SelectAll();
        }

        #endregion Tapping!



        #endregion

    }

}