using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerMenu : ViewModel, IHandle<PlayerDataChanged>
    {
        readonly IEventAggregator eventAggregator;

        public Observable<string> DisplayName;
        public Observable<string> CharacterName;
        public Observable<string> CharacterSummary;
        public Observable<bool> IsMale;
        public Observable<bool> IsFemale;
        public Observable<IDataContext> CharacterClassesViewModel;

        private List<ClassSetViewModel> AvailableCharacterClasses; 

        public PlayerMenu(IEventAggregator eventAggregator) {
            this.eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);  // subscribe to get our player summary change events...


            DisplayName = Observable("DisplayName", "Character Configuration");
            CharacterName = Observable("CharacterName", "");
            CharacterName.ValueChanged += NameChanged;
            CharacterSummary = Observable("CharacterSummary", "");
            IsMale = Observable("IsMale", true);
            IsMale.ValueChanged += CheckedMaleOnValueChanged;
            IsFemale = Observable("IsFemale", true);
            IsFemale.ValueChanged += CheckedFemaleOnValueChanged;

            BuildOutDummyChildContextsForExampleProject();

            CharacterClassesViewModel = Observable("CharacterClassesViewModel", AvailableCharacterClasses[0] as IDataContext); // init and set to male......



            RebuildSummary();
        }

        /// <summary>
        /// In a real scenerio, you would have a proper data Model to populate these child view models.  For the 
        /// sake of this example, we will build out some quick ones here......
        /// </summary>
        private void BuildOutDummyChildContextsForExampleProject() {
            AvailableCharacterClasses = new List<ClassSetViewModel>(2);
            var c0 = (ClassSetViewModel) IoC.GetInstance(typeof(ClassSetViewModel), "ClassSetViewModel");
            c0.ClassNameOne.SetValue("Barbarian Male");
            c0.ClassNameTwo.SetValue("Sorcerer");
            AvailableCharacterClasses.Add(c0);
            var c1 = (ClassSetViewModel) IoC.GetInstance(typeof(ClassSetViewModel), "ClassSetViewModel");
            c1.ClassNameOne.SetValue("Barbarian Female");
            c1.ClassNameTwo.SetValue("Sorceress");
            AvailableCharacterClasses.Add(c1);
        }

        private void NameChanged() {
            RebuildSummary();
        }

        private void RebuildSummary() {
            CharacterSummary.SetValue(
                String.Format("Name: {1}{0}Gender: {2}{0}Class: {3}{0}",
                Environment.NewLine,
                CharacterName.GetValue(),
                IsMale ? "Male" : "Female",
                (this.CharacterClassesViewModel.GetValue() as ClassSetViewModel).SelectedClassName
                )
                );
        }

        private void CheckedFemaleOnValueChanged() {
            updateClassContext();
            RebuildSummary();
        }

        private void CheckedMaleOnValueChanged() {
            updateClassContext(); 
            RebuildSummary();
        }

        private void updateClassContext() {
            if (IsMale.Value) {
                this.CharacterClassesViewModel.SetValue(AvailableCharacterClasses[0]);
            }
            else {
                this.CharacterClassesViewModel.SetValue(AvailableCharacterClasses[1]);
            }
        }

        public void Back() {
            eventAggregator.Publish(new Navigate<MainMenu>());
        }


        public void Handle(PlayerDataChanged message) {
            this.RebuildSummary();
        }
    }
}
