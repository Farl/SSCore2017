using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SS.Core
{
    public class FadeUI : UIBaseSingleton<FadeUI>
    {
        public enum UpdateMethod
        {
            TimeScaled,
            Always,
            Manual
        }
        public enum State
        {
            Start,
            Remain,
            End,
            Finished
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
            private State state = State.Start;
            private bool pause = false;
            private int pauseAt = 0;

            public void Stop()
            {
                state = State.Finished;
            }

            public void Resume()
            {
                pause = false;
                pauseAt = 0;
            }

            public void SetPauseAt(params State[] pauseState)
            {
                foreach (var s in pauseState)
                {
                    pauseAt |= 1 << (int)s;
                }
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
                if (updateMethod != UpdateMethod.Manual)
                {
                    if (start <= 0)
                    {
                        if (remain <= 0)
                        {
                            if (end <= 0)
                            {
                                state = State.Finished;
                            }
                            else
                            {
                                state = State.End;
                            }
                        }
                        else
                        {
                            state = State.Remain;
                        }
                    }
                    else
                    {
                        state = State.Start;
                    }
                }
            }

            public bool Update(ref Color output)
            {
                if (updateMethod != UpdateMethod.Manual)
                {
                    switch (state)
                    {
                        case State.Start:
                            factor = Mathf.Clamp01(timer / start);
                            if (timer >= start)
                            {
                                state += 1;
                                if ((pauseAt & (int)state) != 0)
                                    pause = true;
                            }
                            break;

                        case State.Remain:
                            factor = 1;
                            if (timer >= start + remain)
                            {
                                state += 1;
                                if ((pauseAt & (int)state) != 0)
                                    pause = true;
                            }
                            break;
                        case State.End:
                            factor = 1 - Mathf.Clamp01((timer - start - remain) / end);
                            if (timer >= start + remain + end)
                            {
                                state += 1;
                                if ((pauseAt & (int)state) != 0)
                                    pause = true;
                            }
                            break;
                        case State.Finished:
                            factor = 0;
                            Stop();
                            break;
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

                return state != State.Finished;
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
