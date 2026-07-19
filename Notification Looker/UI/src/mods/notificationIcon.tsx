import { useEffect, useState } from "react";
import styles from "./mainPanel.module.scss";

interface NotificationIconProps {
    iconName: string | undefined;
}

const INVALID_ICONS = new Set([
    "Selected", 
    "Followed",
    "BuildingLevelUp",
    "ThumbsUp",
    "ThumbsDown",
    "ValentineHeart",
]);

export const NotificationIcon = ({ iconName }: NotificationIconProps) => 
{
    const [failed, setFailed] = useState(false);

    useEffect(() => {
        setFailed(false);
    }, [iconName]);

    if (
        !iconName || 
        failed || 
        INVALID_ICONS.has(iconName) || 
        iconName.startsWith("Marker") || 
        iconName.includes("%20Stop") || 
        iconName.includes(" Stop") ||
        iconName.includes("%20Stand") || 
        iconName.includes(" Stand")
    ) 
    {
        return <div className={styles.notificationFallbackIcon}>!</div>;
    }

    return (
        <img 
            className={styles.notificationIcon} 
            src={`Media/Game/Notifications/${iconName}.svg`} 
            onError={() => {setFailed(true)}}
        />
    );
};