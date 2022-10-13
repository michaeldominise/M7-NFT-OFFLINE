using UnityEngine.UIElements;

/// <summary>
/// Factory script for TwoPaneSplitView so that we can use
/// it in the UIBuilder since Unity has not put it in there yet
/// </summary>
public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
}
