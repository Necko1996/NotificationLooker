import { MouseEvent, useEffect, useRef, useState, WheelEvent, useMemo, useCallback } from "react";
import classNames from "classnames";

import { trigger, useValue } from "cs2/api";
import { Panel } from "cs2/ui";

import { bindingMainPanelUISettings, notificationGroupedData, notificationItemData } from "./bindings";
import { ModuleResolver } from "./moduleResolver";
import { UIEventName, UITranslationKey } from "./uiConstants";

import styles from "./mainPanel.module.scss";
import icon from "images/notification-icon.svg";

export const MainPanel = () =>
{
	const mainPanelUISetting = useValue(bindingMainPanelUISettings);

    const notificationsGrouped = useValue(notificationGroupedData);
    const notificationsItem = useValue(notificationItemData);

	const headingText: string = "Notification Looker";

    // Define an element ID for each element that is top level or that needs to be found by ID.
    const elementIDPrefix: string = "notification-looker-";
    const MainPanelID: string = elementIDPrefix + "main-panel";
    const MainPanelCloseID: string = elementIDPrefix + "main-panel-close";

    const notificationListRef = useRef<HTMLDivElement | null>(null);
    const notificationListContentRef = useRef<HTMLDivElement | null>(null);
    const [notificationScrollOffset, setNotificationScrollOffset] = useState(0);
    const [expandedGroup, setExpandedGroup] = useState<string | null>(null);
    const [panelPosition, setPanelPosition] = useState({ x: mainPanelUISetting.mainPanelX, y: mainPanelUISetting.mainPanelX });

    useEffect(() => {
		if (mainPanelUISetting && mainPanelUISetting.mainPanelX > 0 && mainPanelUISetting.mainPanelY > 0) {
			setPanelPosition({ x: mainPanelUISetting.mainPanelX, y: mainPanelUISetting.mainPanelY });
		}
	}, [mainPanelUISetting.mainPanelX, mainPanelUISetting.mainPanelY]);

    // Variables for dragging.
    const dragRef = useRef({
		mainPanel: null as HTMLElement | null,
		relativePositionX: 0,
		relativePositionY: 0
	});

    const itemsMapByGroupName = useMemo(() => {
        const map: Record<string, typeof notificationsItem> = {};
        for (let i = 0; i < notificationsItem.length; i++) {
            const item = notificationsItem[i];
            if (!map[item.name]) {
                map[item.name] = [];
            }
            map[item.name].push(item);
        }
        return map;
    }, [notificationsItem]);

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
    const onMouseDown = useCallback((e: MouseEvent<HTMLDivElement>) => {
		if (e.button !== 0) return;

		const closeButton = document.getElementById(MainPanelCloseID);
		if (closeButton) {
			const closeButtonRect = closeButton.getBoundingClientRect();
			if (e.clientX >= closeButtonRect.left && e.clientX <= closeButtonRect.left + closeButtonRect.width &&
				e.clientY >= closeButtonRect.top && e.clientY <= closeButtonRect.top + closeButtonRect.height) {
				return;
			}
		}

		const panel = document.getElementById(MainPanelID);
		if (panel) {
			const mainPanelRect = panel.getBoundingClientRect();
			
			dragRef.current = {
				mainPanel: panel,
				relativePositionX: e.clientX - mainPanelRect.left,
				relativePositionY: e.clientY - mainPanelRect.top
			};

			window.addEventListener("mousemove", onMouseMove);
			window.addEventListener("mouseup", onMouseUp);

			e.stopPropagation();
			e.preventDefault();
		}
	}, [panelPosition]);

    // Move the main panel while dragging.
    function onMouseMove(e: globalThis.MouseEvent) {
		const { mainPanel, relativePositionX, relativePositionY } = dragRef.current;
		if (mainPanel) {
			const newPositionX = e.clientX - relativePositionX;
			const newPositionY = e.clientY - relativePositionY;

			const mainPanelRect = mainPanel.getBoundingClientRect();
			const checkedPosition = checkPositionOnWindow(newPositionX, newPositionY, mainPanelRect.width, mainPanelRect.height);

			mainPanel.style.left = checkedPosition.x + "px";
			mainPanel.style.top = checkedPosition.y + "px";
		}
	}

    // Finish dragging.
    function onMouseUp(e: globalThis.MouseEvent) {
		if (dragRef.current.mainPanel) {
			window.removeEventListener("mousemove", onMouseMove);
			window.removeEventListener("mouseup", onMouseUp);

			const { mainPanel, relativePositionX, relativePositionY } = dragRef.current;

            const finalX = e.clientX - relativePositionX;
            const finalY = e.clientY - relativePositionY;

            const mainPanelRect = mainPanel.getBoundingClientRect();
            const checkedPosition = checkPositionOnWindow(finalX, finalY, mainPanelRect.width, mainPanelRect.height);

            setPanelPosition(checkedPosition);
            dragRef.current.mainPanel = null;

			trigger(UIEventName.GroupName, UIEventName.MainPanelMoved, checkedPosition.x, checkedPosition.y);
		}
	}

    // Handle click on close button.
    // Click on close button is same as click on activation button.
    function onCloseClick()
    {
        trigger("audio", "playSound", ModuleResolver.instance.UISound.selectItem, 1);
        trigger(UIEventName.GroupName, UIEventName.MainButtonClicked)
    }

    function onInstanceItemClick(entityIndex: number, entityVersion: number)
    {
        trigger("audio", "playSound", ModuleResolver.instance.UISound.selectItem, 1);
        
        trigger(
            UIEventName.GroupName,
            UIEventName.NotificationClicked,
            entityIndex,
            entityVersion
        );
    }

    useEffect(() => {
		return () => {
			window.removeEventListener("mousemove", onMouseMove);
			window.removeEventListener("mouseup", onMouseUp);
		};
	}, []);

	useEffect(() => {
		const maxScroll = getNotificationMaxScroll();
		if (notificationScrollOffset > maxScroll) {
			setNotificationScrollOffset(maxScroll);
		}
	}, [notificationsGrouped.length, expandedGroup, getNotificationMaxScroll]);

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

    const toggleGroupExpand = (groupName: string) => {
        trigger("audio", "playSound", ModuleResolver.instance.UISound.selectItem, 1);

        setExpandedGroup(expandedGroup === groupName ? null : groupName);
    };

	return (
        <>
            {
                mainPanelUISetting.mainPanelShow &&
                (
                    <Panel
                        id={MainPanelID}
                        className={styles.mainPanel}
                        style={{ left: `${panelPosition.x}px`, top: `${panelPosition.y}px` }}
                        header={(
                            <div className={styles.mainPanelHeader} onMouseDown={onMouseDown}>
                                <img className={ModuleResolver.instance.PanelClasses.icon} src={icon} />
                                <div className={classNames(ModuleResolver.instance.PanelThemeClasses.title, styles.mainPanelHeaderTitle)}>
                                    {headingText}
                                </div>
                                <button id={MainPanelCloseID} className={classNames(ModuleResolver.instance.PanelClasses.closeButton, ModuleResolver.instance.RoundHighlightButtonClasses.button, styles.mainPanelHeaderClose)} onClick={onCloseClick}>
                                    <div className={classNames(ModuleResolver.instance.TintedIconClasses.tintedIcon, ModuleResolver.instance.IconButtonClasses.icon)} style={{ maskImage: "url(Media/Glyphs/Close.svg)" }}>
                                    </div>
                                </button>
                            </div>
                        )}
                    >
                        <div
                            ref={notificationListRef}
                            className={styles.notificationList}
                            onWheel={onNotificationWheel}
                        >
                            <div
                                ref={notificationListContentRef}
                                className={styles.notificationListContent}
                                style={{ transform: `translate3d(0, -${notificationScrollOffset}px, 0)` }}
                            >
                            {notificationsGrouped.length === 0 && (
                                <div className={styles.emptyState}>
                                    No notifications found
                                </div>
                            )}

                            {notificationsGrouped.map((group) => {
                                const isExpanded = expandedGroup === group.name;
                                
                                // Local high-performance client-side filtering matching the parent group context name
                                const matchingInstances = itemsMapByGroupName[group.name] || [];

                                return (
                                    <div key={group.name} style={{ display: "flex", flexDirection: "column" }}>
                                        {/* Main Group Header Row */}
                                        <div className={styles.notificationRow} onClick={() => toggleGroupExpand(group.name)}>
                                            <div className={styles.notificationIconBox}>
                                                {group.icon ? (
                                                    <img className={styles.notificationIcon} src={`Media/Game/Notifications/${group.icon}.svg`} />
                                                ) : (
                                                    <div className={styles.notificationFallbackIcon}>!</div>
                                                )}
                                            </div>

                                            <div className={styles.notificationInfo}>
                                                <div className={styles.notificationName}>
                                                    {group.name || "Unknown notification"}
                                                </div>
                                            </div>

                                            <div className={styles.notificationCountBadge}>
                                                {group.count}
                                            </div>
                                        </div>

                                        {/* Child Instance Sub-List */}
                                        {isExpanded && (
                                            <div style={{ display: "flex", flexDirection: "column", paddingLeft: "16px", gap: "2px", marginBottom: "6px", contain: "layout paint style" }}>
                                                {matchingInstances.slice(0, 50).map((instance) => (
                                                    <div 
                                                        className={styles.notificationRow}
                                                        style={{ minHeight: "32px" }} // Slightly slimmer height profile to differentiate sub-items visually
                                                        key={`${instance.entityIndex}:${instance.entityVersion}`}
                                                        onClick={() => onInstanceItemClick(instance.entityIndex, instance.entityVersion)}
                                                    >
                                                        {/* Reusing the exact same indicator box layout for positioning details */}
                                                        <div className={styles.notificationIconBox}>
                                                            {group.icon ? (
                                                                <img className={styles.notificationIcon} src={`Media/Game/Notifications/${instance.icon}.svg`} />
                                                            ) : (
                                                                <div className={styles.notificationFallbackIcon}>!</div>
                                                            )}
                                                        </div>

                                                        <div 
                                                            className={styles.notificationInfo}
                                                            style={{ 
                                                                    display: "flex", 
                                                                    alignItems: "center", 
                                                                    gap: "6px", 
                                                                    whiteSpace: "nowrap", 
                                                                    overflow: "hidden" 
                                                                }}
                                                        >
                                                            <div className={styles.notificationName}>
                                                                Target Instance
                                                            </div>
                                                            <span 
                                                                style={{ 
                                                                    fontSize: "11px", 
                                                                    opacity: 0.5, 
                                                                    fontFamily: "monospace",
                                                                    backgroundColor: "rgba(255, 255, 255, 0.08)",
                                                                    padding: "1px 4px",
                                                                    borderRadius: "4px"
                                                                }}
                                                            >
                                                                #{instance.entityIndex}:{instance.entityVersion}
                                                            </span>
                                                        </div>
                                                    </div>
                                                ))}

                                                {/* Optional indicator if items are hidden */}
                                                {matchingInstances.length > 50 && (
                                                    <div className={styles.emptyState} style={{ padding: "4px", fontSize: "12px", opacity: 0.7 }}>
                                                        Showing first 50 of {matchingInstances.length} instances...
                                                    </div>
                                                )}
                                            </div>
                                        )}
                                    </div>
                                );
                            })}
                            </div>
                        </div>
                    </Panel >
                )
            }
        </>
    )
}