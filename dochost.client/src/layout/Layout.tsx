import {Outlet} from "react-router-dom";
import AuthStatus from "./AuthStatus.tsx";
import {IAuthContext} from "../core/contracts/auth.ts";

type LayoutProps = {
    auth: IAuthContext
}

function Layout( { auth }: LayoutProps) {
    return (
        <div className="">
            <AuthStatus auth={auth}/>
            <Outlet/>
        </div>
    );
}

export default Layout