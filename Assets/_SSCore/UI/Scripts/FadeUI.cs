using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SS
{
    public class FadeUI : UIBaseSingleton<FadeUI>
    {
        public enum UpdateMethod
        {
            TimeScaled,
            Always,
            Manual
        }
        public class FadeTask
        {
            public Color color = Color.black;
            public float start = 0.5f;
            public float remain = 0.5f;
            public float end = 0.5f;
            public UpdateMethod updateMethod = UpdateMethod.TimeScaled;

            private float timer = 0;
            private float startTime;
            private float factor = 0;
            private bool finished = false;
            private bool pause = false;

            public void Stop()
            {
                finished = true;
            }

            public void SetFactor(float f)
            {
                factor = f;
            }

            public void Start()
            {
                startTime = Time.realtimeSinceStartup;
                timer = 0;
                factor = 0;
                finished = false;
            }

            public bool Update(ref Color output)
            {
                if (updateMethod != UpdateMethod.Manual)
                {
                    if (start > 0 && timer < start)
                    {
                        factor = timer / start;
                    }
                    else if (timer <= start + remain)
                    {
                        factor = 1;
                    }
                    else if (end > 0 && timer < start + remain + end)
                    {
                        factor = 1 - (timer - start - remain) / end;
                    }
                    else
                    {
                        factor = 0;
                        Stop();
                    }
                }

                output = new Color(color.r, color.g, color.b, factor);

                if (!pause)
                {
                    if (updateMethod == UpdateMethod.TimeScaled)
                    {
                        timer += Time.deltaTime;
                    }
                    else if (updateMethod == UpdateMethod.Always)
                    {
                        timer += Time.unscaledDeltaTime;
                    }
                }

                return !finished;
            }
        }

        private static List<FadeTask> tasks = new List<FadeTask>();
        private List<FadeTask> removeTask = new List<FadeTask>();

        public Image fadeImage;

        public static FadeTask StartFadeManual(Color color)
        {

            FadeTask fadeTask = new FadeTask()
            {
                color = color,
                updateMethod = UpdateMethod.Manual
            };
            fadeTask.Start();

            tasks.Add(fadeTask);
            return fadeTask;
        }

        public static FadeTask StartFade(Color color, float start, float remain, float end, bool useTimeScale = false)
        {
            FadeTask fadeTask = new FadeTask()
            {
                start = start,
                end = end,
                remain = remain,
                color = color,
                updateMethod = (useTimeScale)? UpdateMethod.TimeScaled: UpdateMethod.Always
            };
            fadeTask.Start();

            tasks.Add(fadeTask);

            return fadeTask;
        }

        private void Start()
        {
            if (fadeImage)
                fadeImage.color = new Color(0, 0, 0, 0);
        }

        private void Update()
        {
            Color color = new Color(0, 0, 0, 0);
            foreach (var task in tasks)
            {
                if (!task.Update(ref color))
                {
                    removeTask.Add(task);
                }
            }

            // Clear
            if (removeTask.Count > 0)
            {
                foreach (var task in removeTask)
                {
                    tasks.Remove(task);
                }
                removeTask.Clear();
            }
            if (tasks.Count <= 0)
            {
                color = new Color(0, 0, 0, 0);
            }

            if (fadeImage)
            {
                fadeImage.color = color;
            }
        }
    }
}
