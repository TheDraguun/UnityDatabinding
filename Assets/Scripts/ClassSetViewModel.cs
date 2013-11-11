using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
	public class ClassSetViewModel : ViewModel
	{
        readonly IEventAggregator eventAggregator;

        public Observable<string> ClassNameOne;
        public Observable<string> ClassNameTwo;
        public Observable<bool> IsClassOne;
        public Observable<bool> IsClassTwo;


	    public String SelectedClassName {
	        get { return IsClassOne ? ClassNameOne.Value : ClassNameTwo.Value; }
	    }

        public ClassSetViewModel(IEventAggregator eventAggregator) {
            this.eventAggregator = eventAggregator;
            ClassNameOne = Observable<string>("ClassNameOne", "");
            ClassNameTwo = Observable<string>("ClassNameTwo", "");
            IsClassOne = Observable<bool>("IsClassOne", true);
            IsClassOne.ValueChanged += IsClassOneOnValueChanged;
            IsClassTwo = Observable<bool>("IsClassTwo", false);
            IsClassTwo.ValueChanged += IsClassTwoOnValueChanged;



        }

	    private void IsClassTwoOnValueChanged() {
            eventAggregator.Publish(new PlayerDataChanged());
	    }

	    private void IsClassOneOnValueChanged() {
            eventAggregator.Publish(new PlayerDataChanged());
	    }
	}
}
