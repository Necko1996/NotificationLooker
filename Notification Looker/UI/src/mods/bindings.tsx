import { bindValue } from "cs2/api";

import { UIEventName } from "./uiConstants";

type NotificationGrouped = {
    entityIndex: number;
    entityVersion: number;
    name: string;
    icon: string;
    count: number;
};

type NotificationItem = { 
    entityIndex: number;
    entityVersion: number;
    name: string;
    icon: string;
};

// Main panel UI settings.
export const bindingMainPanelUISettings = bindValue<boolean>(UIEventName.GroupName, UIEventName.MainPanelUISettings, false);
export const notificationGroupedData = bindValue<NotificationGrouped[]>(UIEventName.GroupName, UIEventName.NotificationGroupedData, []);
export const notificationItemData = bindValue<NotificationItem[]>(UIEventName.GroupName, UIEventName.NotificationItemData, []);