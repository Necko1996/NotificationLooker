import { MouseEvent, useEffect, useRef, useState, WheelEvent } from "react";

import { trigger, useValue } from "cs2/api";
import { Panel } from "cs2/ui";

import { bindingMainPanelUISettings, notificationData } from "./bindings";
import { ModuleResolver } from "./moduleResolver";
import { UIEventName, UITranslationKey } from "./uiConstants";

import styles from "./mainPanel.module.scss";
import icon from "images/notification-icon.svg";

export const MainPanel = () =>
{
	const mainPanelUISetting: boolean = useValue(bindingMainPanelUISettings);

    const notifications = useValue(notificationData);

	const headingText: string = "Notification Looker";

    const elementIDPrefix: string = "notification-looker-";

    // Define an element ID for each element that is top level or that needs to be found by ID.
    const MainPanelID: string = elementIDPrefix + "main-panel";
    const MainPanelCloseID: string = elementIDPrefix + "main-panel-close";

    const notificationListRef = useRef<HTMLDivElement | null>(null);
    const notificationListContentRef = useRef<HTMLDivElement | null>(null);
    const [notificationScrollOffset, setNotificationScrollOffset] = useState(0);

    // Variables for dragging.
    let mainPanel: HTMLElement | null = null;
    let relativePositionX = 0.0;
    let relativePositionY = 0.0;

    // Function to join classes.
    function joinClasses(...classes: any) { return classes.join(" "); }

    function getNotificationMaxScroll(): number
    {
        const list = notificationListRef.current;
        const content = notificationListContentRef.current;

        if (!list || !content)
        {
            return 0;
        }

        return Math.max(0, content.offsetHeight - list.offsetHeight);
    }

    function onNotificationWheel(e: WheelEvent<HTMLDivElement>)
    {
        const maxScroll = getNotificationMaxScroll();

        if (maxScroll <= 0)
        {
            return;
        }

        const delta = e.deltaY;
        const nextOffset = Math.max(0, Math.min(maxScroll, notificationScrollOffset + delta));

        setNotificationScrollOffset(nextOffset);

        e.stopPropagation();
        e.preventDefault();
    }

    // Start dragging.
    // Dragging is initiated by mouse down on the panel header, but it is the whole panel that is moved.
    function onMouseDown(e: MouseEvent<HTMLDivElement, globalThis.MouseEvent>)
    {
        // Ignore mouse down if other than left mouse button.
        if (e.button !== 0)
        {
            return;
        }

        // Get close button.
        const closeButton = document.getElementById(MainPanelCloseID);
        if (closeButton)
        {
            // Ignore mouse down if over the close button.
            const closeButtonRect = closeButton.getBoundingClientRect();
            if (e.clientX >= closeButtonRect.left && e.clientX <= closeButtonRect.left + closeButtonRect.width &&
                e.clientY >= closeButtonRect.top && e.clientY <= closeButtonRect.top + closeButtonRect.height)
            {
                return;
            }
        }

        // Get main panel.
        mainPanel = document.getElementById(MainPanelID);
        if (mainPanel)
        {
            // Save the position of the mouse relative to the main panel.
            const mainPanelRect = mainPanel.getBoundingClientRect();
            relativePositionX = e.clientX - mainPanelRect.left;
            relativePositionY = e.clientY - mainPanelRect.top;

            // Add mouse event listeners.
            window.addEventListener("mousemove", onMouseMove);
            window.addEventListener("mouseup", onMouseUp);

            // Stop event propagation.
            e.stopPropagation();
            e.preventDefault();
        }
    }

    // Move the main panel while dragging.
    function onMouseMove(e: { clientX: number; clientY: number; stopPropagation: () => void; preventDefault: () => void; })
    {
        // Check if main panel is valid.
        if (mainPanel)
        {
            // Compute new panel position based on current mouse position.
            // Adjusting by relative position while dragging keeps the panel in the same location
            // under the pointer as when the panel was originally clicked to start dragging.
            const newPosition = { x: e.clientX - relativePositionX, y: e.clientY - relativePositionY };

            // Prevent any part of panel from going outside the window.
            const mainPanelRect = mainPanel.getBoundingClientRect();
            const checkedPosition = checkPositionOnWindow(newPosition.x, newPosition.y, mainPanelRect.width, mainPanelRect.height);

            // Move panel to checked position.
            mainPanel.style.left = checkedPosition.x + "px";
            mainPanel.style.top  = checkedPosition.y + "px";

            // Stop event propagation.
            e.stopPropagation();
            e.preventDefault();
        }
    }

    // Finish dragging.
    function onMouseUp(e: { stopPropagation: () => void; preventDefault: () => void; })
    {
        // Check if main panel is valid.
        if (mainPanel)
        {
            // Remove mouse event listeners.
            window.removeEventListener("mousemove", onMouseMove);
            window.removeEventListener("mouseup", onMouseUp);

            // Trigger main panel moved event.
            const mainPanelRect = mainPanel.getBoundingClientRect();

            // Stop event propagation.
            e.stopPropagation();
            e.preventDefault();
        }
    }

    // Handle click on close button.
    // Click on close button is same as click on activation button.
    function onCloseClick()
    {
        trigger("audio", "playSound", ModuleResolver.instance.UISound.selectItem, 1);
        trigger(UIEventName.GroupName, UIEventName.MainButtonClicked)
    }

    function onNotificationClick(entityIndex: number, entityVersion: number)
    {
        trigger("audio", "playSound", ModuleResolver.instance.UISound.selectItem, 1);

        trigger(
            UIEventName.GroupName,
            UIEventName.NotificationClicked,
            entityIndex,
            entityVersion
        );
    }

    useEffect(() =>
    {
        const maxScroll = getNotificationMaxScroll();

        if (notificationScrollOffset > maxScroll)
        {
            setNotificationScrollOffset(maxScroll);
        }
    }, [notifications.length, notificationScrollOffset]);

    function checkPositionOnWindow(positionX: number, positionY: number, elementWidth: number, elementHeight: number): {x: number, y: number}
    {
        // Check position against left and top.
        if (positionX < 0) { positionX = 0.0; }
        if (positionY < 0) { positionY = 0.0; }

        // Check position against right and bottom.
        if (positionX > window.innerWidth  - elementWidth ) { positionX = window.innerWidth  - elementWidth;  }
        if (positionY > window.innerHeight - elementHeight) { positionY = window.innerHeight - elementHeight; }

        // Return the checked position.
        return {x: positionX, y: positionY};
    }

	return (
        <>
            {
                mainPanelUISetting &&
                (
                    <Panel
                        id={MainPanelID}
                        className={styles.mainPanel}
                        header={(
                            <div className={styles.mainPanelHeader} onMouseDown={(e) => onMouseDown(e)}>
                                <img className={ModuleResolver.instance.PanelClasses.icon} src={icon} />
                                <div className={joinClasses(ModuleResolver.instance.PanelThemeClasses.title, styles.mainPanelHeaderTitle)}>
                                    {headingText}
                                </div>
                                <button id={MainPanelCloseID} className={joinClasses(ModuleResolver.instance.PanelClasses.closeButton, ModuleResolver.instance.RoundHighlightButtonClasses.button, styles.mainPanelHeaderClose)} onClick={() => onCloseClick()}>
                                    <div className={joinClasses(ModuleResolver.instance.TintedIconClasses.tintedIcon, ModuleResolver.instance.IconButtonClasses.icon)} style={{ maskImage: "url(Media/Glyphs/Close.svg)" }}>
                                    </div>
                                </button>
                            </div>
                        )}
                    >
                        <div
                            ref={notificationListRef}
                            className={styles.notificationList}
                            onWheel={(e) => onNotificationWheel(e)}
                        >
                            <div
                                ref={notificationListContentRef}
                                className={styles.notificationListContent}
                                style={{ transform: `translateY(-${notificationScrollOffset}px)` }}
                            >
                            {notifications.length === 0 && (
                                <div className={styles.emptyState}>
                                    No notifications found
                                </div>
                            )}

                            {notifications.map((item) => (
                                <div className={styles.notificationRow}
                                     key={`${item.entityIndex}:${item.entityVersion}`}
                                     onClick={() => onNotificationClick(item.entityIndex, item.entityVersion)}
                                >
                                    <div className={styles.notificationIconBox}>
                                        {
                                            item.icon ? (
                                                <img className={styles.notificationIcon} src={`Media/Game/Notifications/${item.icon}.svg`} />
                                            ) : (
                                                <div className={styles.notificationFallbackIcon}>
                                                    !
                                                </div>
                                            )
                                        }
                                    </div>

                                    <div className={styles.notificationInfo}>
                                        <div className={styles.notificationName}>
                                            {item.name || "Unknown notification"}
                                        </div>
                                    </div>

                                    <div className={styles.notificationCountBadge}>
                                        {item.count}
                                    </div>
                                </div>
                            ))}
                            </div>
                        </div>
                    </Panel >
                )
            }
        </>
    )
}