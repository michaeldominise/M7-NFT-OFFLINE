using UnityEditor.UIElements;
using UnityEngine.UIElements;

/// <summary>
/// This class controls stack of views
/// </summary>
public class PageController
{
    private ToolbarBreadcrumbs _breadCrumbs;
    private VisualElement _contentContainer;

    /// <summary>
    /// Constructor that pushes an initial content
    /// </summary>
    /// <param name="initialText">The text to be put in the initial breadcrumbs</param>
    /// <param name="breadcrumbs">Breadcrumbs visual element</param>
    /// <param name="contentContainer">Container to put the contents/pages</param>
    public PageController(string initialText, ToolbarBreadcrumbs breadcrumbs, VisualElement contentContainer)
    {
        _breadCrumbs = breadcrumbs;
        _contentContainer = contentContainer;

        _breadCrumbs.PushItem(initialText, PopItemsToHome);
    }

    /// <summary>
    /// Puts a content at the front of the page stack
    /// </summary>
    /// <param name="text">Text to put in bread crumbs</param>
    /// <param name="element">Content visual element</param>
    public void PushItem(string text, VisualElement element)
    {
        _contentContainer.Add(element);
        _breadCrumbs.PushItem(text, () =>
        {
            PopItemsUntil(element);
        });
        ToggleViews(element);
    }

    /// <summary>
    /// Pop/remove all pages until the last one
    /// </summary>
    public void PopItemsToHome()
    {
        for(int i = _contentContainer.childCount - 1; i > 0; i--)
        {
            _contentContainer.RemoveAt(i);
            _breadCrumbs.PopItem();
        }

        ToggleViews(_contentContainer[0]);
    }

    /// <summary>
    /// Pop all items until the <paramref name="element"/>
    /// </summary>
    /// <param name="element"></param>
    public void PopItemsUntil(VisualElement element)
    {
        for (int i = _contentContainer.childCount - 1; i > 0; i--)
        {
            if(_contentContainer[i] == element)
            {
                ToggleViews(element);
                break;
            }
            _contentContainer.RemoveAt(i);
            _breadCrumbs.PopItem();
        }
    }

    /// <summary>
    /// Toggle display of elements hiding or showing them
    /// </summary>
    /// <param name="element"></param>
    private void ToggleViews(VisualElement element)
    {
        foreach(var v in _contentContainer.Children())
        {
            v.style.display = v == element ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
