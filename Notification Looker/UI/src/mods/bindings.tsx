import { bindValue } from "cs2/api";

import { UIEventName } from "./uiConstants";

type NotificationItem = {
    entityIndex: number;
    entityVersion: number;
    name: string;
    icon: string;
    count: number;
};

// Main panel UI settings.
export const bindingMainPanelUISettings = bindValue<boolean>(UIEventName.GroupName, UIEventName.MainPanelUISettings, false);
export const notificationData = bindValue<NotificationItem[]>(UIEventName.GroupName, UIEventName.NotificationData, []);