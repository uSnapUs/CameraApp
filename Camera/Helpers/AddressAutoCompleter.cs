using System;
using System.Linq;

namespace Camera.Helpers
{
    public class AddressAutoCompleter:IAutoCompleteDataSource
    {
        public void PrepareAutoCompleteText(AutoCompleteTextField autoCompleteTextField, string text, bool ignoreCase,
                                            Action<string> onLookedUp)
        {
            StateManager.Current.LocationCoder.LookupLocation(text, locations =>
            {
                var location = locations.FirstOrDefault(l => l.Address.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
                if (location != null)
                {
                    onLookedUp(location.Address.Remove(0, text.Length));
                }
            });
        }
    }
}