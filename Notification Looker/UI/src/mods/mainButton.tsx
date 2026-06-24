import { useValue, trigger } from "cs2/api";
import { Button } from "cs2/ui";

import { bindingMainPanelUISettings } from "./bindings";
import { DescriptionTooltipWithKeyBind } from "./descriptionTooltipWithKeyBind";
import { UIEventName, UITranslationKey } from "./uiConstants";
import icon from "images/notification-icon.svg";

export const MainButton = () => {

    // Get main panel UI settings.
    const mainPanelUISetting = useValue(bindingMainPanelUISettings);

    return (
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
    )
}