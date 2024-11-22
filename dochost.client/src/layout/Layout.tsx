import {Outlet} from "react-router-dom";
import AuthStatus from "./AuthStatus.tsx";

function Layout( { auth }) {
    // TODO: use flex column for the parent div so navigation is always displayed on top
    return (
        <div>
            <AuthStatus auth={auth}/>
            <Outlet/>
        </div>
    );
}

export default Layout