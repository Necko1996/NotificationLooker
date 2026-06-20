import { ModRegistrar } from "cs2/modding";
import { MainButton } from "mods/mainButton";
import { MainPanel } from "mods/mainPanel";

const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.append("GameTopLeft", MainButton);

    moduleRegistry.append("Game", MainPanel);
}

export default register;