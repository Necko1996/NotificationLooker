import { useValue, trigger } from "cs2/api";
import { Button } from "cs2/ui";
import { MouseEvent, useCallback, useRef, useState, useEffect } from "react";

import { bindingMainPanelUISettings, notificationGroupedData } from "./bindings";
import { NotificationIcon } from "./notificationIcon";
import { DescriptionTooltipWithKeyBind } from "./descriptionTooltipWithKeyBind";
import { UIEventName, UITranslationKey } from "./uiConstants";

import styles from "./mainButton.module.scss";
import icon from "images/notification-icon.svg";

export const MainButton = () => {

    // Get main panel UI settings.
    const mainPanelUISetting = useValue(bindingMainPanelUISettings);

    const groupedNotifications = useValue(notificationGroupedData);
    const displayNotifications = groupedNotifications.slice(0, 5);

    const MainButtonContainerID = "notification-looker-main-button-container";

    const [panelPosition, setPanelPosition] = useState({ x: mainPanelUISetting.mainButtonX, y: mainPanelUISetting.mainButtonY });

    useEffect(() => {
		if (mainPanelUISetting && mainPanelUISetting.mainButtonX > 0 && mainPanelUISetting.mainButtonY > 0) {
			setPanelPosition({ x: mainPanelUISetting.mainButtonX, y: mainPanelUISetting.mainButtonY });
		}
	}, [mainPanelUISetting.mainButtonX, mainPanelUISetting.mainButtonY]);

    const dragRef = useRef({
        container: null as HTMLElement | null,
        relativePositionX: 0,
        relativePositionY: 0
    });

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

    function onMouseMove(e: globalThis.MouseEvent) {
        const { container, relativePositionX, relativePositionY } = dragRef.current;
        if (container) {
            const newPositionX = e.clientX - relativePositionX;
            const newPositionY = e.clientY - relativePositionY;

            const rect = container.getBoundingClientRect();
            const checkedPosition = checkPositionOnWindow(newPositionX, newPositionY, rect.width, rect.height);

            container.style.left = checkedPosition.x + "px";
            container.style.top = checkedPosition.y + "px";
        }
    };

    function onMouseUp(e: globalThis.MouseEvent) {
        if (dragRef.current.container) {
            window.removeEventListener("mousemove", onMouseMove);
            window.removeEventListener("mouseup", onMouseUp);

            const { container, relativePositionX, relativePositionY } = dragRef.current;
            const finalX = e.clientX - relativePositionX;
            const finalY = e.clientY - relativePositionY;

            const rect = container.getBoundingClientRect();
            const checkedPosition = checkPositionOnWindow(finalX, finalY, rect.width, rect.height);

            setPanelPosition(checkedPosition);
            dragRef.current.container = null;

            // Trigger the C# binding so settings persist permanently across game loads
            trigger(UIEventName.GroupName, UIEventName.MainButtonMoved, checkedPosition.x, checkedPosition.y);
        }
    };

    const onMouseDown = useCallback((e: MouseEvent<HTMLDivElement>) => {
        if (e.button !== 0) return; // Left click only

        // Prevent dragging if the user is explicitly trying to click an interactive button
        if ((e.target as HTMLElement).tagName === "BUTTON" || (e.target as HTMLElement).closest("button")) {
            return;
        }

        const container = document.getElementById(MainButtonContainerID);
        if (container) {
            const rect = container.getBoundingClientRect();
            
            dragRef.current = {
                container: container,
                relativePositionX: e.clientX - rect.left,
                relativePositionY: e.clientY - rect.top
            };

            window.addEventListener("mousemove", onMouseMove);
            window.addEventListener("mouseup", onMouseUp);

            e.stopPropagation();
            e.preventDefault();
        }
    }, [panelPosition]);

    return (
        <div 
            id={MainButtonContainerID}
            className={styles.nlDraggableWrapper}
            style={{
                left: `${panelPosition.x}px`, 
                top: `${panelPosition.y}px`,
                cursor: "grab",
            }}
            onMouseDown={onMouseDown}
        >
        <div className={styles.nlMainToolbarContainer}>
            <DescriptionTooltipWithKeyBind
                    title="Notification Looker"
                    description="Helps you see all the notification icons cross the city"
                    keyBind="P"
                >
                <Button
                    src={icon}
                    variant="floating"
                    selected={mainPanelUISetting.mainPanelShow}
                    onSelect={() => trigger(UIEventName.GroupName, UIEventName.MainButtonClicked)}
                />
            </DescriptionTooltipWithKeyBind>
            <div className={styles.nlOverviewList}>
                {displayNotifications.map((item) => {
                    const uniqueKey = `${item.entityIndex}-${item.entityVersion}`;
                    return (
                        <div key={uniqueKey} className={styles.nlOverviewCard}>
                            <div className={styles.nlIconContainer}>
                                <NotificationIcon iconName={item.icon} />
                            </div>
                            <div className={styles.nlCountBadge}>
                                <span>{item.count}</span>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
        </div>
    )
}