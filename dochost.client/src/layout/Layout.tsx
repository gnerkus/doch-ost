import {Outlet} from "react-router-dom";
import AuthStatus from "./AuthStatus.tsx";

function Layout() {
    // TODO: use flex column for the parent div so navigation is always displayed on top
    return (
        <div>
            <AuthStatus/>
            <Outlet/>
        </div>
    );
}

export default Layout