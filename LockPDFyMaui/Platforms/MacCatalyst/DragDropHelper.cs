namespace LockPDFyMaui;

using Foundation;
using UIKit;

public static class DragDropHelper
{
    public static void RegisterDrag(UIView view, Func<CancellationToken, Task<Stream>> content)
    {
        var dragInteraction = new UIDragInteraction(new DragInteractionDelegate()
        {
            Content = content
        });
        view.AddInteraction(dragInteraction);
    }

    public static void UnRegisterDrag(UIView view)
    {
        var dragInteractions = view.Interactions.OfType<UIDropInteraction>();
        foreach (var interaction in dragInteractions)
        {
            view.RemoveInteraction(interaction);
        }
    }

    public static void RegisterDrop(UIView view, Func<string, Task>? content)
    {
        var dropInteraction = new UIDropInteraction(new DropInteractionDelegate()
        {
            Content = content
        });
        view.AddInteraction(dropInteraction);
    }

    public static void UnRegisterDrop(UIView view)
    {
        var dropInteractions = view.Interactions.OfType<UIDropInteraction>();
        foreach (var interaction in dropInteractions)
        {
            view.RemoveInteraction(interaction);
        }
    }
}

class DragInteractionDelegate : UIDragInteractionDelegate
{
    public Func<CancellationToken, Task<Stream>>? Content { get; init; }

    public override UIDragItem[] GetItemsForBeginningSession(UIDragInteraction interaction, IUIDragSession session)
    {
        if (Content is null)
        {
            return Array.Empty<UIDragItem>();
        }

        var streamContent = Content.Invoke(CancellationToken.None).GetAwaiter().GetResult();
        var itemProvider = new NSItemProvider(NSData.FromStream(streamContent), UniformTypeIdentifiers.UTTypes.Image.Identifier);
        var dragItem = new UIDragItem(itemProvider);
        return new[] { dragItem };
    }
}

class DropInteractionDelegate : UIDropInteractionDelegate
{
    public Func<string, Task>? Content { get; init; }

    public override UIDropProposal SessionDidUpdate(UIDropInteraction interaction, IUIDropSession session)
    {
        return new UIDropProposal(UIDropOperation.Copy);
    }

    public override void PerformDrop(UIDropInteraction interaction, IUIDropSession session)
    {
        if (Content is null)
        {
            return;
        }

        foreach (var item in session.Items)
        {
            item.ItemProvider.LoadItem(UniformTypeIdentifiers.UTTypes.Json.Identifier, null, async (data, error) =>
            {
                if (data is NSUrl nsData && !string.IsNullOrEmpty(nsData.Path))
                {
                    await Content.Invoke(nsData.Path);
                }
            });
        }
    }
}