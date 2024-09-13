using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine;

namespace SS
{
    // Ref: https://tips.hecomi.com/entry/2022/03/28/235336
    [CustomTimelineEditor(typeof(TimelineSequenceSegment))]
    public class TimelineSequenceSegmentEditor : ClipEditor
    {
        Dictionary<TimelineSequenceSegment, Texture2D> textures = new Dictionary<TimelineSequenceSegment, Texture2D>();
        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);
            var segment = clip.asset as TimelineSequenceSegment;
            if (segment == null)
                return;
            Texture2D texture = GetTexture(segment);
            GUI.DrawTexture(region.position, texture);
        }

        Texture2D GetTexture(TimelineSequenceSegment segment, bool forceUdpate = false)
        {
            Texture2D texture;
            if (forceUdpate)
                textures.Remove(segment);
            if (textures.TryGetValue(segment, out texture) == false || texture == null)
            {
                texture = new Texture2D(64, 1);
                for (int i = 0; i < texture.width; i++)
                {
                    float t = i / (float)texture.width;
                    texture.SetPixel(i, 0, new Color(segment.color.r, segment.color.g, segment.color.b, segment.color.a * (1 - 2 * Mathf.Abs(t - 0.5f))));
                }
                texture.Apply();
                if (!textures.TryAdd(segment, texture))
                {
                    textures[segment] = texture;
                }
            }
            return texture;
        }

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            var segment = clip.asset as TimelineSequenceSegment;
            options.highlightColor = segment.color;
            options.tooltip = segment.speedMultiplier.ToString();
            return options;
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            GetTexture(clip.asset as TimelineSequenceSegment, true);
            base.OnClipChanged(clip);
        }
    }
}
