import {useNavigate} from "react-router-dom";
import {IAuthContext} from "../core/contracts/auth.ts";
import UIIcons from "../core/components/UIIcons.tsx";

type AuthStatusProps = {
    auth: IAuthContext
}

function AuthStatus({auth}: AuthStatusProps) {
    const navigate = useNavigate();

    if (!auth.session) {
        return null;
    }

    return (
        <div className="flex justify-between px-4 py-2 bg-slate-50 items-center shadow h-16">
            <h2 className="text-slate-600 text-2xl">Doc Host</h2>
            <div className="flex gap-4 items-center">
                <UIIcons type="user" />
                <button
                    className="bg-white text-slate-600 border-slate-300 hover:border-slate-500"
                    onClick={() => {
                        auth.logOut(() => navigate("/login"));
                    }}
                >
                    Sign out
                </button>
            </div>
        </div>

    );
}

export default AuthStatus